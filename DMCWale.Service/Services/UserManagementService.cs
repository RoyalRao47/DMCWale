using DMCWale.Data.Models;
using DMCWale.Data.Models.Identity;
using DMCWale.Repo.Interfaces;
using DMCWale.Service.DTOs.Role;
using DMCWale.Service.DTOs.User;
using DMCWale.Service.Interfaces;
using DMCWale.Service.Models.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DMCWale.Service.Services;

public class UserManagementService : IUserManagementService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<ApplicationRole> _roleRepository;
    private readonly IRepository<IdentityUserRole<string>> _userRoleRepository;
    private readonly ILogger<UserManagementService> _logger;

    public UserManagementService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IRepository<User> userRepository,
        IRepository<ApplicationRole> roleRepository,
        IRepository<IdentityUserRole<string>> userRoleRepository,
        ILogger<UserManagementService> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _logger = logger;
    }

    public async Task<ServiceResult> CreateUserAsync(
        CreateUserRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var username = request.Username.Trim();
        var email = request.Email.Trim();
        var mobile = request.Mobile.Trim();
        var agentSupplierCode = request.AgentSupplierCode.Trim().ToUpperInvariant();
        var roleName = request.RoleName.Trim();

        if (string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(mobile) ||
            string.IsNullOrWhiteSpace(agentSupplierCode) ||
            string.IsNullOrWhiteSpace(roleName) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return ServiceResult.Failure("User details are incomplete.");
        }

        if (agentSupplierCode.Length > 50)
        {
            return ServiceResult.Failure("User could not be created.", ["Agent/Supplier Code must be 50 characters or fewer."]);
        }

        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            return ServiceResult.Failure("Selected role is invalid.");
        }

        if (await _userManager.FindByNameAsync(username) is not null)
        {
            return ServiceResult.Failure("User could not be created.", ["Username is already in use."]);
        }

        if (await _userManager.FindByEmailAsync(email) is not null)
        {
            return ServiceResult.Failure("User could not be created.", ["Email is already in use."]);
        }

        if (await _userManager.Users.AnyAsync(user => user.AgentSupplierCode == agentSupplierCode, cancellationToken))
        {
            return ServiceResult.Failure("User could not be created.", ["Agent/Supplier Code is already in use."]);
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
                return ServiceResult.Failure(
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
                IsActive = true,
                IsLeft = false,
                AddDate = DateTime.UtcNow
            };

            await _userRepository.InsertAsync(userProfile, cancellationToken: cancellationToken);

            var roleResult = await _userManager.AddToRoleAsync(applicationUser, roleName);
            if (!roleResult.Succeeded)
            {
                await transaction.RollbackAsync(cancellationToken);
                return ServiceResult.Failure(
                    "User role could not be assigned.",
                    roleResult.Errors.Select(error => error.Description));
            }

            await transaction.CommitAsync(cancellationToken);
            return ServiceResult.Success("User created successfully.");
        }
        catch (Exception exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(exception, "Admin user creation failed for {Username}.", username);
            return ServiceResult.Failure("User could not be created. Please try again.");
        }
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

        var query = _userRepository
            .AsQueryable()
            .AsNoTracking()
            .Join(
                _userRoleRepository.AsQueryable().AsNoTracking(),
                user => user.AspNetUserId,
                userRole => userRole.UserId,
                (user, userRole) => new { user, userRole })
            .Join(
                _roleRepository.AsQueryable().AsNoTracking(),
                x => x.userRole.RoleId,
                role => role.Id,
                (x, role) => new { x.user, role })
            .Where(x => selectedRole == null || x.role.Name == selectedRole)
            .OrderBy(x => x.user.FirstName)
            .ThenBy(x => x.user.LastName)
            .ThenBy(x => x.user.Username)
            .Select(x => new UserListItemDto
            {
                Id = x.user.Id,
                AspNetUserId = x.user.AspNetUserId,
                Username = x.user.Username,
                FirstName = x.user.FirstName,
                LastName = x.user.LastName,
                Email = x.user.Email,
                Mobile = x.user.Mobile,
                IsActive = x.user.IsActive,
                IsLeft = x.user.IsLeft,
                RoleName = x.role.Name ?? string.Empty,
                AddDate = x.user.AddDate,
                ModifyDate = x.user.ModifyDate
            });

        return new UserListResultDto
        {
            Users = await query.ToListAsync(cancellationToken),
            Roles = roles,
            SelectedRole = selectedRole
        };
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
}
