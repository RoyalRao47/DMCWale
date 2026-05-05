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

    public Task<List<UserManagementRecord>> GetUserListAsync(
        string? roleName,
        CancellationToken cancellationToken = default)
    {
        return BuildUserManagementQuery(roleName)
            .OrderBy(user => user.FirstName)
            .ThenBy(user => user.LastName)
            .ThenBy(user => user.Username)
            .ToListAsync(cancellationToken);
    }

    public Task<UserManagementRecord?> GetUserByIdAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        return BuildUserManagementQuery(null)
            .FirstOrDefaultAsync(user => user.Id == userId, cancellationToken);
    }

    public Task<User?> GetUserProfileForUpdateAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        return _context.Set<User>()
            .FirstOrDefaultAsync(user => user.Id == userId, cancellationToken);
    }

    public async Task<bool> AgentSupplierCodeExistsAsync(
        string agentSupplierCode,
        int? excludingUserId = null,
        CancellationToken cancellationToken = default)
    {
        var normalizedCode = agentSupplierCode.Trim().ToUpperInvariant();

        var profileExists = await _context.Set<User>()
            .AsNoTracking()
            .AnyAsync(user =>
                user.AgentSupplierCode == normalizedCode &&
                (!excludingUserId.HasValue || user.Id != excludingUserId.Value),
                cancellationToken);

        if (profileExists)
        {
            return true;
        }

        return await _context.Set<ApplicationUser>()
            .AsNoTracking()
            .GroupJoin(
                _context.Set<User>().AsNoTracking(),
                applicationUser => applicationUser.Id,
                profile => profile.AspNetUserId,
                (applicationUser, profiles) => new { applicationUser, profile = profiles.FirstOrDefault() })
            .AnyAsync(x =>
                x.applicationUser.AgentSupplierCode == normalizedCode &&
                (!excludingUserId.HasValue || x.profile == null || x.profile.Id != excludingUserId.Value),
                cancellationToken);
    }

    public Task<bool> EmailExistsAsync(
        string email,
        int? excludingUserId = null,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToUpperInvariant();

        return _context.Set<ApplicationUser>()
            .AsNoTracking()
            .GroupJoin(
                _context.Set<User>().AsNoTracking(),
                applicationUser => applicationUser.Id,
                profile => profile.AspNetUserId,
                (applicationUser, profiles) => new { applicationUser, profile = profiles.FirstOrDefault() })
            .AnyAsync(x =>
                x.applicationUser.NormalizedEmail == normalizedEmail &&
                (!excludingUserId.HasValue || x.profile == null || x.profile.Id != excludingUserId.Value),
                cancellationToken);
    }

    public Task UpdateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        _context.Set<User>().Update(user);
        return _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ToggleIsActiveAsync(int userId, bool isActive, CancellationToken cancellationToken = default)
    {
        var user = await GetUserProfileForUpdateAsync(userId, cancellationToken);
        if (user is null)
        {
            return false;
        }

        user.IsActive = isActive;
        user.ModifyDate = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ToggleIsLeftAsync(int userId, bool isLeft, CancellationToken cancellationToken = default)
    {
        var user = await GetUserProfileForUpdateAsync(userId, cancellationToken);
        if (user is null)
        {
            return false;
        }

        user.IsLeft = isLeft;
        user.ModifyDate = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private IQueryable<UserManagementRecord> BuildUserManagementQuery(string? roleName)
    {
        return _context.Set<User>()
            .AsNoTracking()
            .Join(
                _context.Set<ApplicationUser>().AsNoTracking(),
                user => user.AspNetUserId,
                applicationUser => applicationUser.Id,
                (user, applicationUser) => new { user, applicationUser })
            .Join(
                _context.Set<IdentityUserRole<string>>().AsNoTracking(),
                x => x.applicationUser.Id,
                userRole => userRole.UserId,
                (x, userRole) => new { x.user, x.applicationUser, userRole })
            .Join(
                _context.Set<ApplicationRole>().AsNoTracking(),
                x => x.userRole.RoleId,
                role => role.Id,
                (x, role) => new { x.user, x.applicationUser, role })
            .Where(x => string.IsNullOrWhiteSpace(roleName) || x.role.Name == roleName)
            .Select(x => new UserManagementRecord
            {
                Id = x.user.Id,
                AspNetUserId = x.user.AspNetUserId,
                Username = x.user.Username,
                FirstName = x.user.FirstName,
                LastName = x.user.LastName,
                Email = x.user.Email,
                Mobile = x.user.Mobile,
                AgentSupplierCode = x.user.AgentSupplierCode != string.Empty
                    ? x.user.AgentSupplierCode
                    : x.applicationUser.AgentSupplierCode ?? string.Empty,
                IsActive = x.user.IsActive,
                IsLeft = x.user.IsLeft,
                RoleName = x.role.Name ?? string.Empty,
                AddDate = x.user.AddDate,
                ModifyDate = x.user.ModifyDate
            });
    }
}
