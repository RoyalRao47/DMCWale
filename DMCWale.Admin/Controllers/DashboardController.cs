using DMCWale.Admin.Filters;
using DMCWale.Data.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMCWale.Admin.Controllers;

[Authorize]
public class DashboardController : Controller
{
    [PagePermissionAuthorize(PagePermissionConstants.DashboardView)]
    public IActionResult Index()
    {
        return View();
    }

    [Authorize(Roles = RoleConstants.Admin)]
    public IActionResult Admin()
    {
        return View();
    }

    [Authorize(Roles = RoleConstants.Agent)]
    public IActionResult Agent()
    {
        return View();
    }

    [Authorize(Roles = RoleConstants.Staff)]
    public IActionResult Staff()
    {
        return View();
    }

    [Authorize(Roles = RoleConstants.Supplier)]
    public IActionResult Supplier()
    {
        return View();
    }
}
