using DMCWale.Admin.ViewModels.Report;
using DMCWale.Data.Constants;
using DMCWale.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DMCWale.Admin.Controllers;

public class ReportController(IReportService reportService) : CrmModuleControllerBase
{
    [HttpGet]
    public Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        return ReportPageAsync("Reports", cancellationToken);
    }

    [HttpGet]
    public Task<IActionResult> LeadConversion(CancellationToken cancellationToken)
    {
        return ReportPageAsync("Lead Conversion Report", cancellationToken);
    }

    [HttpGet]
    public Task<IActionResult> Booking(CancellationToken cancellationToken)
    {
        return ReportPageAsync("Booking Report", cancellationToken);
    }

    [HttpGet]
    public Task<IActionResult> Payment(CancellationToken cancellationToken)
    {
        return ReportPageAsync("Payment Report", cancellationToken);
    }

    [HttpGet]
    public Task<IActionResult> AgentPerformance(CancellationToken cancellationToken)
    {
        return ReportPageAsync("Agent Performance Report", cancellationToken);
    }

    [HttpGet]
    public Task<IActionResult> SupplierPerformance(CancellationToken cancellationToken)
    {
        return ReportPageAsync("Supplier Performance Report", cancellationToken);
    }

    private async Task<IActionResult> ReportPageAsync(string title, CancellationToken cancellationToken)
    {
        if (!await CanAsync(reportService, CrmPermissionActionConstants.View, cancellationToken))
        {
            return Forbid();
        }

        return View(ControllerContext.ActionDescriptor.ActionName, FeatureModel<ReportViewModel>(reportService.ModuleCode, title, "Report"));
    }
}
