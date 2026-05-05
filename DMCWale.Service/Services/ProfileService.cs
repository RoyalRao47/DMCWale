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

        await using var transaction = await _userRepository
            .GetDbContext()
            .Database
            .BeginTransactionAsync(cancellationToken);

        try
        {
            applicationUser.PhoneNumber = request.Mobile.Trim();

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

            userProfile.FirstName = request.FirstName.Trim();
            userProfile.LastName = request.LastName.Trim();
            userProfile.Mobile = request.Mobile.Trim();
            userProfile.ModifyDate = DateTime.UtcNow;

            userDetail.Salutation = request.Salutation?.Trim() ?? string.Empty;
            userDetail.CountryCode = request.CountryCode?.Trim() ?? string.Empty;
            userDetail.City = request.City?.Trim() ?? string.Empty;
            userDetail.Address1 = request.Address1?.Trim() ?? string.Empty;
            userDetail.Address2 = request.Address2?.Trim() ?? string.Empty;
            userDetail.Signature = request.Signature?.Trim() ?? string.Empty;
            userDetail.ModifyDate = DateTime.UtcNow;

            await _userRepository.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            await _signInManager.RefreshSignInAsync(applicationUser);

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
