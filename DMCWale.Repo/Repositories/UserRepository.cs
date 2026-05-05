using DMCWale.Data;
using DMCWale.Data.Models;
using DMCWale.Data.Models.Identity;
using DMCWale.Repo.Interfaces;
using DMCWale.Repo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DMCWale.Repo.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<AuthUserRecord?> GetByEmailAndAgentSupplierCodeAsync(
        string normalizedEmailOrUserName,
        string agentSupplierCode,
        CancellationToken cancellationToken = default)
    {
        return _context.Set<ApplicationUser>()
            .AsNoTracking()
            .Where(user =>
                (user.NormalizedEmail == normalizedEmailOrUserName ||
                 user.NormalizedUserName == normalizedEmailOrUserName) &&
                user.AgentSupplierCode == agentSupplierCode)
            .GroupJoin(
                _context.Set<User>().AsNoTracking(),
                user => user.Id,
                profile => profile.AspNetUserId,
                (user, profiles) => new { user, profile = profiles.FirstOrDefault() })
            .Join(
                _context.Set<IdentityUserRole<string>>().AsNoTracking(),
                x => x.user.Id,
                userRole => userRole.UserId,
                (x, userRole) => new { x.user, x.profile, userRole })
            .Join(
                _context.Set<ApplicationRole>().AsNoTracking(),
                x => x.userRole.RoleId,
                role => role.Id,
                (x, role) => new AuthUserRecord
                {
                    ApplicationUser = x.user,
                    UserProfile = x.profile,
                    RoleName = role.Name ?? string.Empty
                })
            .FirstOrDefaultAsync(cancellationToken);
    }
}
