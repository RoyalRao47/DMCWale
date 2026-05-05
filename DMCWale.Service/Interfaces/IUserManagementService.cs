using DMCWale.Service.DTOs.Role;
using DMCWale.Service.DTOs.User;
using DMCWale.Service.Models.Common;

namespace DMCWale.Service.Interfaces;

public interface IUserManagementService
{
    Task<CreateUserResultDto> CreateUserAsync(CreateUserDto request, CancellationToken cancellationToken = default);

    Task<UserListResultDto> GetUsersAsync(UserFilterRequestDto request, CancellationToken cancellationToken = default);

    Task<EditUserDto?> GetUserForEditAsync(int userId, CancellationToken cancellationToken = default);

    Task<OperationResultDto> UpdateUserAsync(EditUserDto request, CancellationToken cancellationToken = default);

    Task<OperationResultDto> ToggleUserActiveStatusAsync(int userId, bool isActive, CancellationToken cancellationToken = default);

    Task<OperationResultDto> ToggleUserLeftStatusAsync(int userId, bool isLeft, CancellationToken cancellationToken = default);

    Task<string> GenerateUniqueAgentSupplierCodeAsync(CancellationToken cancellationToken = default);

    Task<List<RoleDto>> GetRolesAsync(CancellationToken cancellationToken = default);
}
