using DMCWale.Data.Constants;
using DMCWale.Admin.ViewModels.User;
using DMCWale.Service.DTOs.User;
using DMCWale.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DMCWale.Admin.Controllers;

[Authorize(Roles = RoleConstants.Admin)]
public class UserController : Controller
{
    private readonly IUserManagementService _userManagementService;

    public UserController(IUserManagementService userManagementService)
    {
        _userManagementService = userManagementService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? roleName, CancellationToken cancellationToken)
    {
        var result = await _userManagementService.GetUsersAsync(
            new UserFilterRequestDto { RoleName = roleName },
            cancellationToken);

        var model = new UserListViewModel
        {
            RoleName = result.SelectedRole,
            Roles = BuildRoleSelectList(result.Roles.Select(role => role.Name), result.SelectedRole),
            Users = result.Users.Select(user => new UserListItemViewModel
            {
                Id = user.Id,
                AspNetUserId = user.AspNetUserId,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Mobile = user.Mobile,
                IsActive = user.IsActive,
                IsLeft = user.IsLeft,
                RoleName = user.RoleName,
                AddDate = user.AddDate,
                ModifyDate = user.ModifyDate
            }).ToList()
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var model = new CreateUserViewModel
        {
            Roles = await GetRoleSelectListAsync(cancellationToken: cancellationToken)
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUserViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            model.Roles = await GetRoleSelectListAsync(model.RoleName, cancellationToken);
            return View(model);
        }

        var result = await _userManagementService.CreateUserAsync(
            new CreateUserRequestDto
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Username = model.Username,
                Email = model.Email,
                Mobile = model.Mobile,
                Password = model.Password,
                RoleName = model.RoleName
            },
            cancellationToken);

        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }

        model.Roles = await GetRoleSelectListAsync(model.RoleName, cancellationToken);
        return View(model);
    }

    private async Task<List<SelectListItem>> GetRoleSelectListAsync(
        string? selectedRole = null,
        CancellationToken cancellationToken = default)
    {
        var roles = await _userManagementService.GetRolesAsync(cancellationToken);
        return BuildRoleSelectList(roles.Select(role => role.Name), selectedRole);
    }

    private static List<SelectListItem> BuildRoleSelectList(IEnumerable<string> roles, string? selectedRole)
    {
        return roles
            .Where(role => !string.IsNullOrWhiteSpace(role))
            .Select(role => new SelectListItem
            {
                Text = role,
                Value = role,
                Selected = role == selectedRole
            })
            .ToList();
    }
}
