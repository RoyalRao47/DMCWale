using DMCWale.Data.Models;
using DMCWale.Data.Models.Identity;
using DMCWale.Repo.Interfaces;
using DMCWale.Service.DTOs.Role;
using DMCWale.Service.DTOs.User;
using DMCWale.Service.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DMCWale.Service.Services;

public class UserManagementService : IUserManagementService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IUserRepository _managementUserRepository;
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<ApplicationRole> _roleRepository;
    private readonly ILogger<UserManagementService> _logger;

    public UserManagementService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IUserRepository managementUserRepository,
        IRepository<User> userRepository,
        IRepository<ApplicationRole> roleRepository,
        ILogger<UserManagementService> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _managementUserRepository = managementUserRepository;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _logger = logger;
    }

    public async Task<CreateUserResultDto> CreateUserAsync(
        CreateUserDto request,
        CancellationToken cancellationToken = default)
    {
        var username = request.Username.Trim();
        var email = request.Email.Trim();
        var mobile = request.Mobile.Trim();
        var roleName = request.RoleName.Trim();

        if (string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(mobile) ||
            string.IsNullOrWhiteSpace(roleName) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return CreateFailure("User details are incomplete.");
        }

        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            return CreateFailure("Selected role is invalid.");
        }

        if (await _userManager.FindByNameAsync(username) is not null)
        {
            return CreateFailure("User could not be created.", ["Username is already in use."]);
        }

        if (await _userManager.FindByEmailAsync(email) is not null)
        {
            return CreateFailure("User could not be created.", ["Email is already in use."]);
        }

        for (var attempt = 0; attempt < 5; attempt++)
        {
            var agentSupplierCode = await GenerateUniqueAgentSupplierCodeAsync(cancellationToken);
            if (await _managementUserRepository.AgentSupplierCodeExistsAsync(agentSupplierCode, cancellationToken: cancellationToken))
            {
                continue;
            }

            await using var transaction = await _userRepository.GetDbContext().Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var applicationUser = new ApplicationUser
                {
                    UserName = username,
                    Email = email,
                    AgentSupplierCode = agentSupplierCode,
                    PhoneNumber = mobile,
                    EmailConfirmed = true
                };

                var identityResult = await _userManager.CreateAsync(applicationUser, request.Password);
                if (!identityResult.Succeeded)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return CreateFailure(
                        "User could not be created.",
                        identityResult.Errors.Select(error => error.Description));
                }

                var userProfile = new User
                {
                    AspNetUserId = applicationUser.Id,
                    Username = username,
                    FirstName = request.FirstName.Trim(),
                    LastName = request.LastName.Trim(),
                    Email = email,
                    Mobile = mobile,
                    AgentSupplierCode = agentSupplierCode,
                    IsActive = true,
                    IsLeft = false,
                    AddDate = DateTime.UtcNow
                };

                await _userRepository.InsertAsync(userProfile, cancellationToken: cancellationToken);

                var roleResult = await _userManager.AddToRoleAsync(applicationUser, roleName);
                if (!roleResult.Succeeded)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return CreateFailure(
                        "User role could not be assigned.",
                        roleResult.Errors.Select(error => error.Description));
                }

                await transaction.CommitAsync(cancellationToken);
                return new CreateUserResultDto
                {
                    Succeeded = true,
                    Message = "User created successfully.",
                    UserId = userProfile.Id,
                    AgentSupplierCode = agentSupplierCode
                };
            }
            catch (DbUpdateException exception) when (IsUniqueConstraintException(exception))
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogWarning(exception, "Generated Agent/Supplier Code {Code} conflicted. Retrying.", agentSupplierCode);
            }
            catch (Exception exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(exception, "Admin user creation failed for {Username}.", username);
                return CreateFailure("User could not be created. Please try again.");
            }
        }

        return CreateFailure("User could not be created.", ["Could not generate a unique Agent/Supplier Code. Please try again."]);
    }

    public async Task<UserListResultDto> GetUsersAsync(
        UserFilterRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var selectedRole = string.IsNullOrWhiteSpace(request.RoleName)
            ? null
            : request.RoleName.Trim();

        var roles = await GetRolesAsync(cancellationToken);
        if (selectedRole is not null && roles.All(role => role.Name != selectedRole))
        {
            return new UserListResultDto
            {
                Roles = roles,
                SelectedRole = selectedRole
            };
        }

        var users = await _managementUserRepository.GetUserListAsync(selectedRole, cancellationToken);

        return new UserListResultDto
        {
            Users = users.Select(user => new UserListItemDto
            {
                Id = user.Id,
                AspNetUserId = user.AspNetUserId,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Mobile = user.Mobile,
                AgentSupplierCode = user.AgentSupplierCode,
                IsActive = user.IsActive,
                IsLeft = user.IsLeft,
                RoleName = user.RoleName,
                AddDate = user.AddDate,
                ModifyDate = user.ModifyDate
            }).ToList(),
            Roles = roles,
            SelectedRole = selectedRole
        };
    }

    public async Task<EditUserDto?> GetUserForEditAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        var user = await _managementUserRepository.GetUserByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return null;
        }

        return new EditUserDto
        {
            Id = user.Id,
            AspNetUserId = user.AspNetUserId,
            Username = user.Username,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Mobile = user.Mobile,
            AgentSupplierCode = user.AgentSupplierCode,
            RoleName = user.RoleName,
            IsActive = user.IsActive,
            IsLeft = user.IsLeft
        };
    }

    public async Task<OperationResultDto> UpdateUserAsync(
        EditUserDto request,
        CancellationToken cancellationToken = default)
    {
        var profile = await _managementUserRepository.GetUserProfileForUpdateAsync(request.Id, cancellationToken);
        if (profile is null)
        {
            return OperationResultDto.Failure("User was not found.");
        }

        var applicationUser = await _userManager.FindByIdAsync(profile.AspNetUserId);
        if (applicationUser is null)
        {
            return OperationResultDto.Failure("Identity user was not found.");
        }

        var firstName = request.FirstName.Trim();
        var lastName = request.LastName.Trim();
        var username = request.Username.Trim();
        var email = request.Email.Trim();
        var mobile = request.Mobile.Trim();
        var roleName = request.RoleName.Trim();
        var existingCode = profile.AgentSupplierCode;

        if (string.IsNullOrWhiteSpace(firstName) ||
            string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(mobile) ||
            string.IsNullOrWhiteSpace(roleName))
        {
            return OperationResultDto.Failure("User details are incomplete.");
        }

        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            return OperationResultDto.Failure("Selected role is invalid.");
        }

        if (await _managementUserRepository.EmailExistsAsync(email, request.Id, cancellationToken))
        {
            return OperationResultDto.Failure("User could not be updated.", ["Email is already in use."]);
        }

        if (await _managementUserRepository.AgentSupplierCodeExistsAsync(existingCode, request.Id, cancellationToken))
        {
            return OperationResultDto.Failure("User could not be updated.", ["Agent/Supplier Code is already in use."]);
        }

        await using var transaction = await _userRepository.GetDbContext().Database.BeginTransactionAsync(cancellationToken);

        try
        {
            applicationUser.UserName = username;
            applicationUser.Email = email;
            applicationUser.PhoneNumber = mobile;
            applicationUser.AgentSupplierCode = existingCode;

            var updateIdentityResult = await _userManager.UpdateAsync(applicationUser);
            if (!updateIdentityResult.Succeeded)
            {
                await transaction.RollbackAsync(cancellationToken);
                return OperationResultDto.Failure(
                    "User could not be updated.",
                    updateIdentityResult.Errors.Select(error => error.Description));
            }

            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(applicationUser);
                var resetPasswordResult = await _userManager.ResetPasswordAsync(applicationUser, resetToken, request.Password);
                if (!resetPasswordResult.Succeeded)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return OperationResultDto.Failure(
                        "Password could not be updated.",
                        resetPasswordResult.Errors.Select(error => error.Description));
                }
            }

            var currentRoles = await _userManager.GetRolesAsync(applicationUser);
            var rolesToRemove = currentRoles.Where(role => role != roleName).ToList();
            if (rolesToRemove.Count > 0)
            {
                var removeRoleResult = await _userManager.RemoveFromRolesAsync(applicationUser, rolesToRemove);
                if (!removeRoleResult.Succeeded)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return OperationResultDto.Failure(
                        "User role could not be updated.",
                        removeRoleResult.Errors.Select(error => error.Description));
                }
            }

            if (!currentRoles.Contains(roleName))
            {
                var addRoleResult = await _userManager.AddToRoleAsync(applicationUser, roleName);
                if (!addRoleResult.Succeeded)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return OperationResultDto.Failure(
                        "User role could not be updated.",
                        addRoleResult.Errors.Select(error => error.Description));
                }
            }

            profile.Username = username;
            profile.FirstName = firstName;
            profile.LastName = lastName;
            profile.Email = email;
            profile.Mobile = mobile;
            profile.AgentSupplierCode = existingCode;
            profile.IsActive = request.IsActive;
            profile.IsLeft = request.IsLeft;
            profile.ModifyDate = DateTime.UtcNow;

            await _managementUserRepository.UpdateUserAsync(profile, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return OperationResultDto.Success("User updated successfully.");
        }
        catch (DbUpdateException exception) when (IsUniqueConstraintException(exception))
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogWarning(exception, "User update failed because a unique value conflicted for {UserId}.", request.Id);
            return OperationResultDto.Failure("User could not be updated.", ["Email, username, or Agent/Supplier Code is already in use."]);
        }
        catch (Exception exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(exception, "Admin user update failed for {UserId}.", request.Id);
            return OperationResultDto.Failure("User could not be updated. Please try again.");
        }
    }

    public async Task<OperationResultDto> ToggleUserActiveStatusAsync(
        int userId,
        bool isActive,
        CancellationToken cancellationToken = default)
    {
        var updated = await _managementUserRepository.ToggleIsActiveAsync(userId, isActive, cancellationToken);
        return updated
            ? OperationResultDto.Success("User active status updated.")
            : OperationResultDto.Failure("User was not found.");
    }

    public async Task<OperationResultDto> ToggleUserLeftStatusAsync(
        int userId,
        bool isLeft,
        CancellationToken cancellationToken = default)
    {
        var updated = await _managementUserRepository.ToggleIsLeftAsync(userId, isLeft, cancellationToken);
        return updated
            ? OperationResultDto.Success("User left status updated.")
            : OperationResultDto.Failure("User was not found.");
    }

    public async Task<string> GenerateUniqueAgentSupplierCodeAsync(CancellationToken cancellationToken = default)
    {
        for (var attempt = 0; attempt < 100; attempt++)
        {
            var number = Random.Shared.Next(1000, 10000);
            var code = $"DMC-{number}";

            if (!await _managementUserRepository.AgentSupplierCodeExistsAsync(code, cancellationToken: cancellationToken))
            {
                return code;
            }
        }

        throw new InvalidOperationException("Could not generate a unique Agent/Supplier Code.");
    }

    public Task<List<RoleDto>> GetRolesAsync(CancellationToken cancellationToken = default)
    {
        return _roleRepository
            .AsQueryable()
            .AsNoTracking()
            .OrderBy(role => role.Name)
            .Select(role => new RoleDto
            {
                Id = role.Id,
                Name = role.Name ?? string.Empty
            })
            .ToListAsync(cancellationToken);
    }

    private static CreateUserResultDto CreateFailure(string message, IEnumerable<string>? errors = null)
    {
        return new CreateUserResultDto
        {
            Succeeded = false,
            Message = message,
            Errors = errors?.ToList() ?? [message]
        };
    }

    private static bool IsUniqueConstraintException(DbUpdateException exception)
    {
        return exception.InnerException?.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase) == true ||
            exception.InnerException?.Message.Contains("unique", StringComparison.OrdinalIgnoreCase) == true;
    }
}
