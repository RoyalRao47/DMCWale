using DMCWale.Admin.ViewModels.Quotation;
using DMCWale.Data.Constants;
using DMCWale.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DMCWale.Admin.Controllers;

public class QuotationController(IQuotationService quotationService) : CrmModuleControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        if (!await CanAsync(quotationService, CrmPermissionActionConstants.View, cancellationToken))
        {
            return Forbid();
        }

        return View(Map<QuotationViewModel>(await quotationService.GetListAsync(cancellationToken)));
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        if (!await CanAsync(quotationService, CrmPermissionActionConstants.View, cancellationToken))
        {
            return Forbid();
        }

        return View(Map<QuotationViewModel>(await quotationService.GetDetailsAsync(id, cancellationToken)));
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        if (!await CanAsync(quotationService, CrmPermissionActionConstants.Add, cancellationToken))
        {
            return Forbid();
        }

        return View(Map<QuotationViewModel>(await quotationService.GetFormAsync(cancellationToken: cancellationToken)));
    }

    [HttpGet("/Quotation/Create/{packageId:int}")]
    public async Task<IActionResult> CreateFromPackage(int packageId, CancellationToken cancellationToken)
    {
        if (!await CanAsync(quotationService, CrmPermissionActionConstants.Add, cancellationToken))
        {
            return Forbid();
        }

        return View("Feature", FeatureModel<QuotationViewModel>(quotationService.ModuleCode, "Create Quotation From Package", "Quotation"));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(QuotationViewModel model, CancellationToken cancellationToken)
    {
        if (!await CanAsync(quotationService, CrmPermissionActionConstants.Add, cancellationToken))
        {
            return Forbid();
        }

        var result = await quotationService.SaveAsync(ToSaveRequest(model), cancellationToken);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        AddErrors(result.Errors);
        return View(await RebuildFormAsync<QuotationViewModel>(quotationService, model, cancellationToken: cancellationToken));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        if (!await CanAsync(quotationService, CrmPermissionActionConstants.Edit, cancellationToken))
        {
            return Forbid();
        }

        var model = Map<QuotationViewModel>(await quotationService.GetFormAsync(id, cancellationToken));
        model.Rows = [new() { Id = id }];
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, QuotationViewModel model, CancellationToken cancellationToken)
    {
        if (!await CanAsync(quotationService, CrmPermissionActionConstants.Edit, cancellationToken))
        {
            return Forbid();
        }

        var request = ToSaveRequest(model);
        request.Id = id;
        var result = await quotationService.SaveAsync(request, cancellationToken);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        AddErrors(result.Errors);
        return View(await RebuildFormAsync<QuotationViewModel>(quotationService, model, id, cancellationToken));
    }

    [HttpGet("/Quotation/Preview/{id:int}")]
    public async Task<IActionResult> Preview(int id, CancellationToken cancellationToken)
    {
        if (!await CanAsync(quotationService, CrmPermissionActionConstants.View, cancellationToken))
        {
            return Forbid();
        }

        return View("Feature", FeatureModel<QuotationViewModel>(quotationService.ModuleCode, "Quotation Preview", "Quotation"));
    }

    [HttpGet("/Quotation/Send/{id:int}")]
    public async Task<IActionResult> Send(int id, CancellationToken cancellationToken)
    {
        if (!await CanAsync(quotationService, CrmPermissionActionConstants.Edit, cancellationToken))
        {
            return Forbid();
        }

        return View("Feature", FeatureModel<QuotationViewModel>(quotationService.ModuleCode, "Send Quotation", "Quotation"));
    }

    [HttpGet("/Quotation/VersionHistory/{id:int}")]
    public async Task<IActionResult> VersionHistory(int id, CancellationToken cancellationToken)
    {
        if (!await CanAsync(quotationService, CrmPermissionActionConstants.View, cancellationToken))
        {
            return Forbid();
        }

        return View("Feature", FeatureModel<QuotationViewModel>(quotationService.ModuleCode, "Quotation Version History", "Quotation"));
    }

    [HttpGet("/Quotation/Voucher/{id:int}")]
    public async Task<IActionResult> Voucher(int id, CancellationToken cancellationToken)
    {
        if (!await CanAsync(quotationService, CrmPermissionActionConstants.Export, cancellationToken))
        {
            return Forbid();
        }

        return View("Feature", FeatureModel<QuotationViewModel>(quotationService.ModuleCode, "Quotation Voucher", "Quotation"));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        if (!await CanAsync(quotationService, CrmPermissionActionConstants.Delete, cancellationToken))
        {
            return Forbid();
        }

        var result = await quotationService.SoftDeleteAsync(id, cancellationToken);
        TempData[result.Succeeded ? "SuccessMessage" : "ErrorMessage"] = result.Succeeded
            ? result.Message
            : string.Join(" ", result.Errors);
        return RedirectToAction(nameof(Index));
    }
}
