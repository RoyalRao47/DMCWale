using DMCWale.Data.Constants;
using DMCWale.Service.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMCWale.Admin.Controllers;

[Authorize(Roles = RoleConstants.Admin)]
public class RoleController : Controller
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    public async Task<IActionResult> Index()
    {
        var roles = await _roleService.GetAllRolesAsync();
        return View(roles.Select(role => role.Name ?? string.Empty).ToList());
    }
}
