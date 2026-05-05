using DMCWale.Repo.Models;
using DMCWale.Data.Models;

namespace DMCWale.Repo.Interfaces;

public interface IUserRepository
{
    Task<AuthUserRecord?> GetByEmailAndAgentSupplierCodeAsync(
        string normalizedEmailOrUserName,
        string agentSupplierCode,
        CancellationToken cancellationToken = default);

    Task<List<UserManagementRecord>> GetUserListAsync(
        string? roleName,
        CancellationToken cancellationToken = default);

    Task<UserManagementRecord?> GetUserByIdAsync(
        int userId,
        CancellationToken cancellationToken = default);

    Task<User?> GetUserProfileForUpdateAsync(
        int userId,
        CancellationToken cancellationToken = default);

    Task<bool> AgentSupplierCodeExistsAsync(
        string agentSupplierCode,
        int? excludingUserId = null,
        CancellationToken cancellationToken = default);

    Task<bool> EmailExistsAsync(
        string email,
        int? excludingUserId = null,
        CancellationToken cancellationToken = default);

    Task UpdateUserAsync(User user, CancellationToken cancellationToken = default);

    Task<bool> ToggleIsActiveAsync(int userId, bool isActive, CancellationToken cancellationToken = default);

    Task<bool> ToggleIsLeftAsync(int userId, bool isLeft, CancellationToken cancellationToken = default);
}
