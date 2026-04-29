using DMCWale.Service.Models.Common;
using DMCWale.Data.Models.Identity;

namespace DMCWale.Service.Services.Interfaces;

public interface IRoleService
{
    Task<IReadOnlyList<ApplicationRole>> GetAllRolesAsync();

    Task<ServiceResult> CreateRoleAsync(string roleName);

    Task<bool> RoleExistsAsync(string roleName);

    Task<ServiceResult> AssignRoleIfNeededAsync(ApplicationUser user, string roleName);
}
