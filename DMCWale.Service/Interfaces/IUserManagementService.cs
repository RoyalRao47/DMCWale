using DMCWale.Service.DTOs.Role;
using DMCWale.Service.DTOs.User;
using DMCWale.Service.Models.Common;

namespace DMCWale.Service.Interfaces;

public interface IUserManagementService
{
    Task<ServiceResult> CreateUserAsync(CreateUserRequestDto request, CancellationToken cancellationToken = default);

    Task<UserListResultDto> GetUsersAsync(UserFilterRequestDto request, CancellationToken cancellationToken = default);

    Task<List<RoleDto>> GetRolesAsync(CancellationToken cancellationToken = default);
}
