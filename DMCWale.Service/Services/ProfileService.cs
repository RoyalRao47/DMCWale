using DMCWale.Data.Models;
using DMCWale.Data.Models.Identity;
using DMCWale.Repo.Interfaces;
using DMCWale.Service.DTOs.Profile;
using DMCWale.Service.Interfaces;
using DMCWale.Service.Models.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DMCWale.Service.Services;

public class ProfileService : IProfileService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<UserDetail> _userDetailRepository;
    private readonly ILogger<ProfileService> _logger;

    public ProfileService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IRepository<User> userRepository,
        IRepository<UserDetail> userDetailRepository,
        ILogger<ProfileService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _userRepository = userRepository;
        _userDetailRepository = userDetailRepository;
        _logger = logger;
    }

    public async Task<ProfileDetailsDto?> GetProfileAsync(
        string aspNetUserId,
        CancellationToken cancellationToken = default)
    {
        var applicationUser = await _userManager.FindByIdAsync(aspNetUserId);
        if (applicationUser is null)
        {
            return null;
        }

        var userProfile = await _userRepository
            .AsQueryable()
            .FirstOrDefaultAsync(user => user.AspNetUserId == aspNetUserId, cancellationToken);

        var userDetail = await _userDetailRepository
            .AsQueryable()
            .FirstOrDefaultAsync(detail => detail.AspNetUserId == aspNetUserId, cancellationToken);

        var roles = await _userManager.GetRolesAsync(applicationUser);

        return new ProfileDetailsDto
        {
            FirstName = userProfile?.FirstName ?? string.Empty,
            LastName = userProfile?.LastName ?? string.Empty,
            Mobile = userProfile?.Mobile ?? applicationUser.PhoneNumber ?? string.Empty,
            Salutation = userDetail?.Salutation ?? "Mr.",
            CountryCode = userDetail?.CountryCode ?? string.Empty,
            City = userDetail?.City ?? string.Empty,
            Address1 = userDetail?.Address1 ?? string.Empty,
            Address2 = userDetail?.Address2 ?? string.Empty,
            Signature = userDetail?.Signature ?? string.Empty,
            Username = userProfile?.Username ?? applicationUser.UserName ?? string.Empty,
            Email = userProfile?.Email ?? applicationUser.Email ?? string.Empty,
            AgentSupplierCode = applicationUser.AgentSupplierCode ?? userProfile?.AgentSupplierCode ?? string.Empty,
            RoleName = roles.FirstOrDefault() ?? string.Empty,
            ProfileImagePath = userDetail?.ProfileImagePath
        };
    }

    public async Task<ServiceResult> UpdateProfileAsync(
        string aspNetUserId,
        UpdateProfileRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var applicationUser = await _userManager.FindByIdAsync(aspNetUserId);
        if (applicationUser is null)
        {
            return ServiceResult.Failure("User account was not found.");
        }

        var firstName = request.FirstName.Trim();
        var lastName = request.LastName.Trim();
        var email = string.IsNullOrWhiteSpace(request.Email)
            ? applicationUser.Email?.Trim() ?? string.Empty
            : request.Email.Trim();
        var mobile = request.Mobile.Trim();
        var city = request.City.Trim();
        var address = request.Address1.Trim();
        var validationErrors = new List<string>();

        if (string.IsNullOrWhiteSpace(firstName))
        {
            validationErrors.Add("First name is required.");
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            validationErrors.Add("Last name is required.");
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            validationErrors.Add("Email is required.");
        }
        else if (!new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(email))
        {
            validationErrors.Add("Enter a valid email address.");
        }

        if (string.IsNullOrWhiteSpace(mobile))
        {
            validationErrors.Add("Mobile is required.");
        }
        else if (mobile.Length > 30)
        {
            validationErrors.Add("Mobile must be 30 characters or fewer.");
        }

        if (string.IsNullOrWhiteSpace(city))
        {
            validationErrors.Add("City is required.");
        }

        if (string.IsNullOrWhiteSpace(address))
        {
            validationErrors.Add("Address is required.");
        }

        if (validationErrors.Count > 0)
        {
            return ServiceResult.Failure("Profile details are incomplete.", validationErrors);
        }

        var normalizedEmail = _userManager.NormalizeEmail(email);
        var emailOwner = await _userManager.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(
                user => user.NormalizedEmail == normalizedEmail && user.Id != aspNetUserId,
                cancellationToken);

        if (emailOwner is not null)
        {
            return ServiceResult.Failure("Profile could not be updated.", ["Email is already in use."]);
        }

        await using var transaction = await _userRepository
            .GetDbContext()
            .Database
            .BeginTransactionAsync(cancellationToken);

        try
        {
            applicationUser.Email = email;
            applicationUser.NormalizedEmail = normalizedEmail;
            applicationUser.PhoneNumber = mobile;

            var identityResult = await _userManager.UpdateAsync(applicationUser);
            if (!identityResult.Succeeded)
            {
                await transaction.RollbackAsync(cancellationToken);
                return ServiceResult.Failure(
                    "Profile could not be updated.",
                    identityResult.Errors.Select(error => error.Description));
            }

            var userProfile = await GetOrCreateUserProfileAsync(applicationUser, cancellationToken);
            var userDetail = await GetOrCreateUserDetailAsync(aspNetUserId, cancellationToken);

            userProfile.FirstName = firstName;
            userProfile.LastName = lastName;
            userProfile.Email = email;
            userProfile.Mobile = mobile;
            userProfile.ModifyDate = DateTime.UtcNow;

            userDetail.Salutation = request.Salutation?.Trim() ?? string.Empty;
            userDetail.CountryCode = request.CountryCode?.Trim() ?? string.Empty;
            userDetail.City = city;
            userDetail.Address1 = address;
            userDetail.Address2 = request.Address2?.Trim() ?? string.Empty;
            userDetail.Signature = request.Signature?.Trim() ?? string.Empty;
            userDetail.ModifyDate = DateTime.UtcNow;

            await _userRepository.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            if (_signInManager.Context.User?.Identity?.AuthenticationType == IdentityConstants.ApplicationScheme)
            {
                await _signInManager.RefreshSignInAsync(applicationUser);
            }

            return ServiceResult.Success("Profile updated successfully.");
        }
        catch (Exception exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(exception, "Profile update failed for AspNetUserId {AspNetUserId}.", aspNetUserId);
            return ServiceResult.Failure("Profile could not be updated. Please try again.");
        }
    }

    public async Task<ServiceResult> ChangePasswordAsync(string aspNetUserId, ChangePasswordRequestDto request)
    {
        var applicationUser = await _userManager.FindByIdAsync(aspNetUserId);
        if (applicationUser is null)
        {
            return ServiceResult.Failure("User account was not found.");
        }

        var result = await _userManager.ChangePasswordAsync(
            applicationUser,
            request.OldPassword,
            request.NewPassword);

        if (!result.Succeeded)
        {
            return ServiceResult.Failure(
                "Password could not be changed.",
                result.Errors.Select(error => error.Description));
        }

        await _signInManager.RefreshSignInAsync(applicationUser);
        return ServiceResult.Success("Password changed successfully.");
    }

    public async Task<ServiceResult> UpdateProfileImagePathAsync(
        string aspNetUserId,
        string profileImagePath,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userDetail = await GetOrCreateUserDetailAsync(aspNetUserId, cancellationToken);
            userDetail.ProfileImagePath = profileImagePath;
            userDetail.ModifyDate = DateTime.UtcNow;

            await _userDetailRepository.SaveChangesAsync(cancellationToken);
            return ServiceResult.Success("Profile photo updated successfully.");
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Profile photo update failed for AspNetUserId {AspNetUserId}.", aspNetUserId);
            return ServiceResult.Failure("Profile photo could not be updated. Please try again.");
        }
    }

    private async Task<User> GetOrCreateUserProfileAsync(
        ApplicationUser applicationUser,
        CancellationToken cancellationToken)
    {
        var userProfile = await _userRepository
            .Query()
            .AsTracking()
            .Filter(user => user.AspNetUserId == applicationUser.Id)
            .GetQueryable()
            .FirstOrDefaultAsync(cancellationToken);

        if (userProfile is not null)
        {
            return userProfile;
        }

        applicationUser.AgentSupplierCode ??= await GenerateUniqueAgentSupplierCodeAsync(cancellationToken);
        await _userManager.UpdateAsync(applicationUser);

        userProfile = new User
        {
            AspNetUserId = applicationUser.Id,
            Username = applicationUser.UserName ?? string.Empty,
            FirstName = string.Empty,
            LastName = string.Empty,
            Email = applicationUser.Email ?? string.Empty,
            Mobile = applicationUser.PhoneNumber ?? string.Empty,
            AgentSupplierCode = applicationUser.AgentSupplierCode,
            IsActive = true,
            IsLeft = false,
            AddDate = DateTime.UtcNow
        };

        await _userRepository.InsertAsync(userProfile, saveChanges: false, cancellationToken);
        return userProfile;
    }

    private async Task<UserDetail> GetOrCreateUserDetailAsync(
        string aspNetUserId,
        CancellationToken cancellationToken)
    {
        var userDetail = await _userDetailRepository
            .Query()
            .AsTracking()
            .Filter(detail => detail.AspNetUserId == aspNetUserId)
            .GetQueryable()
            .FirstOrDefaultAsync(cancellationToken);

        if (userDetail is not null)
        {
            return userDetail;
        }

        userDetail = new UserDetail
        {
            AspNetUserId = aspNetUserId,
            Salutation = "Mr.",
            AddDate = DateTime.UtcNow
        };

        await _userDetailRepository.InsertAsync(userDetail, saveChanges: false, cancellationToken);
        return userDetail;
    }

    private async Task<string> GenerateUniqueAgentSupplierCodeAsync(CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < 100; attempt++)
        {
            var code = $"DMC-{Random.Shared.Next(1000, 10000)}";
            if (!await _userManager.Users.AnyAsync(user => user.AgentSupplierCode == code, cancellationToken))
            {
                return code;
            }
        }

        throw new InvalidOperationException("Could not generate a unique Agent/Supplier Code.");
    }
}
