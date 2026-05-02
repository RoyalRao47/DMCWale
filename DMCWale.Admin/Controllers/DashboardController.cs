using DMCWale.Admin.Filters;
using DMCWale.Admin.ViewModels.Crm;
using DMCWale.Data.Constants;
using DMCWale.Service.DTOs.Crm;
using DMCWale.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMCWale.Admin.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly ICrmModuleService _crmModuleService;

    public DashboardController(ICrmModuleService crmModuleService)
    {
        _crmModuleService = crmModuleService;
    }

    [PagePermissionAuthorize(PagePermissionConstants.DashboardView)]
    public IActionResult Index()
    {
        return View();
    }

    [Authorize(Roles = RoleConstants.Admin)]
    public async Task<IActionResult> Admin(CancellationToken cancellationToken)
    {
        var dashboard = await _crmModuleService.GetDashboardAsync(RoleConstants.Admin, cancellationToken);
        return View(Map(dashboard));
    }

    [Authorize(Roles = RoleConstants.Agent)]
    public async Task<IActionResult> Agent(CancellationToken cancellationToken)
    {
        var dashboard = await _crmModuleService.GetDashboardAsync(RoleConstants.Agent, cancellationToken);
        return View(Map(dashboard));
    }

    [Authorize(Roles = RoleConstants.Staff)]
    public async Task<IActionResult> Staff(CancellationToken cancellationToken)
    {
        var dashboard = await _crmModuleService.GetDashboardAsync(RoleConstants.Staff, cancellationToken);
        return View(Map(dashboard));
    }

    [Authorize(Roles = RoleConstants.Supplier)]
    public async Task<IActionResult> Supplier(CancellationToken cancellationToken)
    {
        var dashboard = await _crmModuleService.GetDashboardAsync(RoleConstants.Supplier, cancellationToken);
        return View(Map(dashboard));
    }

    private static CrmDashboardViewModel Map(CrmDashboardDto dto)
    {
        return new CrmDashboardViewModel
        {
            Title = dto.Title,
            Metrics = dto.Metrics.Select(metric => new CrmMetricViewModel
            {
                Label = metric.Label,
                Value = metric.Value
            }).ToList()
        };
    }
}
