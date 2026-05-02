using DMCWale.Admin.ViewModels.Lead;
using DMCWale.Data.Constants;
using DMCWale.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DMCWale.Admin.Controllers;

public class LeadController(ILeadService leadService) : CrmModuleControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        if (!await CanAsync(leadService, CrmPermissionActionConstants.View, cancellationToken))
        {
            return Forbid();
        }

        return View(Map<LeadViewModel>(await leadService.GetListAsync(cancellationToken)));
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        if (!await CanAsync(leadService, CrmPermissionActionConstants.View, cancellationToken))
        {
            return Forbid();
        }

        return View(Map<LeadViewModel>(await leadService.GetDetailsAsync(id, cancellationToken)));
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        if (!await CanAsync(leadService, CrmPermissionActionConstants.Add, cancellationToken))
        {
            return Forbid();
        }

        return View(Map<LeadViewModel>(await leadService.GetFormAsync(cancellationToken: cancellationToken)));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LeadViewModel model, CancellationToken cancellationToken)
    {
        if (!await CanAsync(leadService, CrmPermissionActionConstants.Add, cancellationToken))
        {
            return Forbid();
        }

        var result = await leadService.SaveAsync(ToSaveRequest(model), cancellationToken);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        AddErrors(result.Errors);
        return View(await RebuildFormAsync<LeadViewModel>(leadService, model, cancellationToken: cancellationToken));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        if (!await CanAsync(leadService, CrmPermissionActionConstants.Edit, cancellationToken))
        {
            return Forbid();
        }

        var model = Map<LeadViewModel>(await leadService.GetFormAsync(id, cancellationToken));
        model.Rows = [new() { Id = id }];
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, LeadViewModel model, CancellationToken cancellationToken)
    {
        if (!await CanAsync(leadService, CrmPermissionActionConstants.Edit, cancellationToken))
        {
            return Forbid();
        }

        var request = ToSaveRequest(model);
        request.Id = id;
        var result = await leadService.SaveAsync(request, cancellationToken);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        AddErrors(result.Errors);
        return View(await RebuildFormAsync<LeadViewModel>(leadService, model, id, cancellationToken));
    }

    [HttpGet("/Lead/Validate/{id:int}")]
    public async Task<IActionResult> Validate(int id, CancellationToken cancellationToken)
    {
        if (!await CanAsync(leadService, CrmPermissionActionConstants.Approve, cancellationToken))
        {
            return Forbid();
        }

        return View("Feature", FeatureModel<LeadViewModel>(leadService.ModuleCode, "Validate Lead", "Lead"));
    }

    [HttpGet("/Lead/Assign/{id:int}")]
    public async Task<IActionResult> Assign(int id, CancellationToken cancellationToken)
    {
        if (!await CanAsync(leadService, CrmPermissionActionConstants.Assign, cancellationToken))
        {
            return Forbid();
        }

        return View("Feature", FeatureModel<LeadViewModel>(leadService.ModuleCode, "Assign Lead", "Lead"));
    }

    [HttpGet("/Lead/History/{id:int}")]
    public async Task<IActionResult> History(int id, CancellationToken cancellationToken)
    {
        if (!await CanAsync(leadService, CrmPermissionActionConstants.View, cancellationToken))
        {
            return Forbid();
        }

        return View("Feature", FeatureModel<LeadViewModel>(leadService.ModuleCode, "Lead Assignment History", "Lead"));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        if (!await CanAsync(leadService, CrmPermissionActionConstants.Delete, cancellationToken))
        {
            return Forbid();
        }

        var result = await leadService.SoftDeleteAsync(id, cancellationToken);
        TempData[result.Succeeded ? "SuccessMessage" : "ErrorMessage"] = result.Succeeded
            ? result.Message
            : string.Join(" ", result.Errors);
        return RedirectToAction(nameof(Index));
    }
}
