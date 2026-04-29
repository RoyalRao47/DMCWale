using DMCWale.Service.Models.Common;
using DMCWale.Data.Models.Identity;
using DMCWale.Service.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DMCWale.Service.Services;

public class RoleService : IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public RoleService(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task<IReadOnlyList<ApplicationRole>> GetAllRolesAsync()
    {
        return await _roleManager.Roles
            .OrderBy(role => role.Name)
            .ToListAsync();
    }

    public async Task<ServiceResult> CreateRoleAsync(string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
        {
            return ServiceResult.Failure("Role name is required.");
        }

        if (await _roleManager.RoleExistsAsync(roleName))
        {
            return ServiceResult.Success("Role already exists.");
        }

        var result = await _roleManager.CreateAsync(new ApplicationRole { Name = roleName.Trim() });
        return result.Succeeded
            ? ServiceResult.Success("Role created successfully.")
            : ServiceResult.Failure("Role could not be created.", result.Errors.Select(error => error.Description));
    }

    public Task<bool> RoleExistsAsync(string roleName)
    {
        return _roleManager.RoleExistsAsync(roleName);
    }

    public async Task<ServiceResult> AssignRoleIfNeededAsync(ApplicationUser user, string roleName)
    {
        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            return ServiceResult.Failure("Selected role does not exist.");
        }

        if (await _userManager.IsInRoleAsync(user, roleName))
        {
            return ServiceResult.Success("User already has this role.");
        }

        var result = await _userManager.AddToRoleAsync(user, roleName);
        return result.Succeeded
            ? ServiceResult.Success("Role assigned successfully.")
            : ServiceResult.Failure("Role could not be assigned.", result.Errors.Select(error => error.Description));
    }
}
