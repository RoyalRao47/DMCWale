using DMCWale.Service.DTOs.Profile;
using DMCWale.Service.Models.Common;

namespace DMCWale.Service.Interfaces;

public interface IProfileService
{
    Task<ProfileDetailsDto?> GetProfileAsync(string aspNetUserId, CancellationToken cancellationToken = default);

    Task<ServiceResult> UpdateProfileAsync(
        string aspNetUserId,
        UpdateProfileRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ServiceResult> ChangePasswordAsync(string aspNetUserId, ChangePasswordRequestDto request);

    Task<ServiceResult> UpdateProfileImagePathAsync(
        string aspNetUserId,
        string profileImagePath,
        CancellationToken cancellationToken = default);
}
