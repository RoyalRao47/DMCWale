using DMCWale.Repo.Models;

namespace DMCWale.Repo.Interfaces;

public interface IUserRepository
{
    Task<AuthUserRecord?> GetByEmailAndAgentSupplierCodeAsync(
        string normalizedEmailOrUserName,
        string agentSupplierCode,
        CancellationToken cancellationToken = default);
}
