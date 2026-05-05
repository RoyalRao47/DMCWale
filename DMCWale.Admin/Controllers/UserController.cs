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
                AgentSupplierCode = user.AgentSupplierCode,
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
            new CreateUserDto
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

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var user = await _userManagementService.GetUserForEditAsync(id, cancellationToken);
        if (user is null)
        {
            return NotFound();
        }

        return View(new EditUserViewModel
        {
            Id = user.Id,
            AspNetUserId = user.AspNetUserId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Username = user.Username,
            Email = user.Email,
            Mobile = user.Mobile,
            AgentSupplierCode = user.AgentSupplierCode,
            RoleName = user.RoleName,
            IsActive = user.IsActive,
            IsLeft = user.IsLeft,
            Roles = await GetRoleSelectListAsync(user.RoleName, cancellationToken)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditUserViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            model.Roles = await GetRoleSelectListAsync(model.RoleName, cancellationToken);
            return View(model);
        }

        var result = await _userManagementService.UpdateUserAsync(
            new EditUserDto
            {
                Id = model.Id,
                AspNetUserId = model.AspNetUserId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Username = model.Username,
                Email = model.Email,
                Mobile = model.Mobile,
                AgentSupplierCode = model.AgentSupplierCode,
                Password = model.Password,
                RoleName = model.RoleName,
                IsActive = model.IsActive,
                IsLeft = model.IsLeft
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

        var currentUser = await _userManagementService.GetUserForEditAsync(model.Id, cancellationToken);
        if (currentUser is not null)
        {
            model.AgentSupplierCode = currentUser.AgentSupplierCode;
        }

        model.Password = null;
        model.ConfirmPassword = null;
        model.Roles = await GetRoleSelectListAsync(model.RoleName, cancellationToken);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive([FromBody] ToggleUserStatusViewModel model, CancellationToken cancellationToken)
    {
        var result = await _userManagementService.ToggleUserActiveStatusAsync(model.UserId, model.IsActive, cancellationToken);
        return Json(new
        {
            success = result.Succeeded,
            message = result.Succeeded ? result.Message : string.Join(" ", result.Errors),
            value = model.IsActive
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleLeft([FromBody] ToggleUserStatusViewModel model, CancellationToken cancellationToken)
    {
        var result = await _userManagementService.ToggleUserLeftStatusAsync(model.UserId, model.IsLeft, cancellationToken);
        return Json(new
        {
            success = result.Succeeded,
            message = result.Succeeded ? result.Message : string.Join(" ", result.Errors),
            value = model.IsLeft
        });
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
