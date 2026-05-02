using DMCWale.Admin.ViewModels.Currency;
using DMCWale.Data.Constants;
using DMCWale.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DMCWale.Admin.Controllers;

public class CurrencyController(ICurrencyService currencyService) : CrmModuleControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        if (!await CanAsync(currencyService, CrmPermissionActionConstants.View, cancellationToken))
        {
            return Forbid();
        }

        return View(Map<CurrencyViewModel>(await currencyService.GetListAsync(cancellationToken)));
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        if (!await CanAsync(currencyService, CrmPermissionActionConstants.View, cancellationToken))
        {
            return Forbid();
        }

        return View(Map<CurrencyViewModel>(await currencyService.GetDetailsAsync(id, cancellationToken)));
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        if (!await CanAsync(currencyService, CrmPermissionActionConstants.Add, cancellationToken))
        {
            return Forbid();
        }

        return View(Map<CurrencyViewModel>(await currencyService.GetFormAsync(cancellationToken: cancellationToken)));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CurrencyViewModel model, CancellationToken cancellationToken)
    {
        if (!await CanAsync(currencyService, CrmPermissionActionConstants.Add, cancellationToken))
        {
            return Forbid();
        }

        var result = await currencyService.SaveAsync(ToSaveRequest(model), cancellationToken);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        AddErrors(result.Errors);
        return View(await RebuildFormAsync<CurrencyViewModel>(currencyService, model, cancellationToken: cancellationToken));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        if (!await CanAsync(currencyService, CrmPermissionActionConstants.Edit, cancellationToken))
        {
            return Forbid();
        }

        var model = Map<CurrencyViewModel>(await currencyService.GetFormAsync(id, cancellationToken));
        model.Rows = [new() { Id = id }];
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CurrencyViewModel model, CancellationToken cancellationToken)
    {
        if (!await CanAsync(currencyService, CrmPermissionActionConstants.Edit, cancellationToken))
        {
            return Forbid();
        }

        var request = ToSaveRequest(model);
        request.Id = id;
        var result = await currencyService.SaveAsync(request, cancellationToken);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        AddErrors(result.Errors);
        return View(await RebuildFormAsync<CurrencyViewModel>(currencyService, model, id, cancellationToken));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        if (!await CanAsync(currencyService, CrmPermissionActionConstants.Delete, cancellationToken))
        {
            return Forbid();
        }

        var result = await currencyService.SoftDeleteAsync(id, cancellationToken);
        TempData[result.Succeeded ? "SuccessMessage" : "ErrorMessage"] = result.Succeeded
            ? result.Message
            : string.Join(" ", result.Errors);
        return RedirectToAction(nameof(Index));
    }
}
