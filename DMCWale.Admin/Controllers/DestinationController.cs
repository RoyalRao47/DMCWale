using DMCWale.Admin.ViewModels.Destination;
using DMCWale.Data.Constants;
using DMCWale.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DMCWale.Admin.Controllers;

public class DestinationController(IDestinationService destinationService) : CrmModuleControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        if (!await CanAsync(destinationService, CrmPermissionActionConstants.View, cancellationToken))
        {
            return Forbid();
        }

        return View(Map<DestinationViewModel>(await destinationService.GetListAsync(cancellationToken)));
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        if (!await CanAsync(destinationService, CrmPermissionActionConstants.View, cancellationToken))
        {
            return Forbid();
        }

        return View(Map<DestinationViewModel>(await destinationService.GetDetailsAsync(id, cancellationToken)));
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        if (!await CanAsync(destinationService, CrmPermissionActionConstants.Add, cancellationToken))
        {
            return Forbid();
        }

        return View(Map<DestinationViewModel>(await destinationService.GetFormAsync(cancellationToken: cancellationToken)));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DestinationViewModel model, CancellationToken cancellationToken)
    {
        if (!await CanAsync(destinationService, CrmPermissionActionConstants.Add, cancellationToken))
        {
            return Forbid();
        }

        var result = await destinationService.SaveAsync(ToSaveRequest(model), cancellationToken);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        AddErrors(result.Errors);
        return View(await RebuildFormAsync<DestinationViewModel>(destinationService, model, cancellationToken: cancellationToken));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        if (!await CanAsync(destinationService, CrmPermissionActionConstants.Edit, cancellationToken))
        {
            return Forbid();
        }

        var model = Map<DestinationViewModel>(await destinationService.GetFormAsync(id, cancellationToken));
        model.Rows = [new() { Id = id }];
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, DestinationViewModel model, CancellationToken cancellationToken)
    {
        if (!await CanAsync(destinationService, CrmPermissionActionConstants.Edit, cancellationToken))
        {
            return Forbid();
        }

        var request = ToSaveRequest(model);
        request.Id = id;
        var result = await destinationService.SaveAsync(request, cancellationToken);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        AddErrors(result.Errors);
        return View(await RebuildFormAsync<DestinationViewModel>(destinationService, model, id, cancellationToken));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        if (!await CanAsync(destinationService, CrmPermissionActionConstants.Delete, cancellationToken))
        {
            return Forbid();
        }

        var result = await destinationService.SoftDeleteAsync(id, cancellationToken);
        TempData[result.Succeeded ? "SuccessMessage" : "ErrorMessage"] = result.Succeeded
            ? result.Message
            : string.Join(" ", result.Errors);
        return RedirectToAction(nameof(Index));
    }
}
