using DMCWale.Data.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMCWale.Admin.Controllers;

[Authorize(Roles = RoleConstants.Admin)]
public class RolePermissionController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return RedirectToAction("Index", "PageClaims");
    }

    [HttpGet]
    public IActionResult Edit(string roleId)
    {
        return RedirectToAction("Index", "PageClaims");
    }
}
