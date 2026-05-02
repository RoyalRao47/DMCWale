using DMCWale.Admin.ViewModels.Execution;
using DMCWale.Data.Constants;
using DMCWale.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DMCWale.Admin.Controllers;

public class ExecutionController(IExecutionService executionService) : CrmModuleControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        if (!await CanAsync(executionService, CrmPermissionActionConstants.View, cancellationToken))
        {
            return Forbid();
        }

        return View(Map<ExecutionViewModel>(await executionService.GetListAsync(cancellationToken)));
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        if (!await CanAsync(executionService, CrmPermissionActionConstants.View, cancellationToken))
        {
            return Forbid();
        }

        return View(Map<ExecutionViewModel>(await executionService.GetDetailsAsync(id, cancellationToken)));
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        if (!await CanAsync(executionService, CrmPermissionActionConstants.Add, cancellationToken))
        {
            return Forbid();
        }

        return View(Map<ExecutionViewModel>(await executionService.GetFormAsync(cancellationToken: cancellationToken)));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ExecutionViewModel model, CancellationToken cancellationToken)
    {
        if (!await CanAsync(executionService, CrmPermissionActionConstants.Add, cancellationToken))
        {
            return Forbid();
        }

        var result = await executionService.SaveAsync(ToSaveRequest(model), cancellationToken);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        AddErrors(result.Errors);
        return View(await RebuildFormAsync<ExecutionViewModel>(executionService, model, cancellationToken: cancellationToken));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        if (!await CanAsync(executionService, CrmPermissionActionConstants.Edit, cancellationToken))
        {
            return Forbid();
        }

        var model = Map<ExecutionViewModel>(await executionService.GetFormAsync(id, cancellationToken));
        model.Rows = [new() { Id = id }];
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ExecutionViewModel model, CancellationToken cancellationToken)
    {
        if (!await CanAsync(executionService, CrmPermissionActionConstants.Edit, cancellationToken))
        {
            return Forbid();
        }

        var request = ToSaveRequest(model);
        request.Id = id;
        var result = await executionService.SaveAsync(request, cancellationToken);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        AddErrors(result.Errors);
        return View(await RebuildFormAsync<ExecutionViewModel>(executionService, model, id, cancellationToken));
    }

    [HttpGet("/Execution/UpdateStatus/{bookingId:int}")]
    public async Task<IActionResult> UpdateStatus(int bookingId, CancellationToken cancellationToken)
    {
        if (!await CanAsync(executionService, CrmPermissionActionConstants.Execute, cancellationToken))
        {
            return Forbid();
        }

        return View("Feature", FeatureModel<ExecutionViewModel>(executionService.ModuleCode, "Update Execution Status", "Execution"));
    }

    [HttpGet("/Execution/Complete/{bookingId:int}")]
    public async Task<IActionResult> Complete(int bookingId, CancellationToken cancellationToken)
    {
        if (!await CanAsync(executionService, CrmPermissionActionConstants.Execute, cancellationToken))
        {
            return Forbid();
        }

        return View("Feature", FeatureModel<ExecutionViewModel>(executionService.ModuleCode, "Complete Execution", "Execution"));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        if (!await CanAsync(executionService, CrmPermissionActionConstants.Delete, cancellationToken))
        {
            return Forbid();
        }

        var result = await executionService.SoftDeleteAsync(id, cancellationToken);
        TempData[result.Succeeded ? "SuccessMessage" : "ErrorMessage"] = result.Succeeded
            ? result.Message
            : string.Join(" ", result.Errors);
        return RedirectToAction(nameof(Index));
    }
}
