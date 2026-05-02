using DMCWale.Admin.ViewModels.Payment;
using DMCWale.Data.Constants;
using DMCWale.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DMCWale.Admin.Controllers;

public class PaymentController(IPaymentService paymentService) : CrmModuleControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        if (!await CanAsync(paymentService, CrmPermissionActionConstants.View, cancellationToken))
        {
            return Forbid();
        }

        return View(Map<PaymentViewModel>(await paymentService.GetListAsync(cancellationToken)));
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        if (!await CanAsync(paymentService, CrmPermissionActionConstants.View, cancellationToken))
        {
            return Forbid();
        }

        return View(Map<PaymentViewModel>(await paymentService.GetDetailsAsync(id, cancellationToken)));
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        if (!await CanAsync(paymentService, CrmPermissionActionConstants.Add, cancellationToken))
        {
            return Forbid();
        }

        return View(Map<PaymentViewModel>(await paymentService.GetFormAsync(cancellationToken: cancellationToken)));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PaymentViewModel model, CancellationToken cancellationToken)
    {
        if (!await CanAsync(paymentService, CrmPermissionActionConstants.Add, cancellationToken))
        {
            return Forbid();
        }

        var result = await paymentService.SaveAsync(ToSaveRequest(model), cancellationToken);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        AddErrors(result.Errors);
        return View(await RebuildFormAsync<PaymentViewModel>(paymentService, model, cancellationToken: cancellationToken));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        if (!await CanAsync(paymentService, CrmPermissionActionConstants.Edit, cancellationToken))
        {
            return Forbid();
        }

        var model = Map<PaymentViewModel>(await paymentService.GetFormAsync(id, cancellationToken));
        model.Rows = [new() { Id = id }];
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PaymentViewModel model, CancellationToken cancellationToken)
    {
        if (!await CanAsync(paymentService, CrmPermissionActionConstants.Edit, cancellationToken))
        {
            return Forbid();
        }

        var request = ToSaveRequest(model);
        request.Id = id;
        var result = await paymentService.SaveAsync(request, cancellationToken);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        AddErrors(result.Errors);
        return View(await RebuildFormAsync<PaymentViewModel>(paymentService, model, id, cancellationToken));
    }

    [HttpGet("/Payment/CreateLink/{bookingId:int}")]
    public async Task<IActionResult> CreateLink(int bookingId, CancellationToken cancellationToken)
    {
        if (!await CanAsync(paymentService, CrmPermissionActionConstants.Add, cancellationToken))
        {
            return Forbid();
        }

        return View("Feature", FeatureModel<PaymentViewModel>(paymentService.ModuleCode, "Create Payment Link", "Payment"));
    }

    [HttpGet("/Payment/RecordManual/{bookingId:int}")]
    public async Task<IActionResult> RecordManual(int bookingId, CancellationToken cancellationToken)
    {
        if (!await CanAsync(paymentService, CrmPermissionActionConstants.RecordPayment, cancellationToken))
        {
            return Forbid();
        }

        return View("Feature", FeatureModel<PaymentViewModel>(paymentService.ModuleCode, "Record Manual Payment", "Payment"));
    }

    [HttpGet("/Payment/History/{bookingId:int}")]
    public async Task<IActionResult> History(int bookingId, CancellationToken cancellationToken)
    {
        if (!await CanAsync(paymentService, CrmPermissionActionConstants.View, cancellationToken))
        {
            return Forbid();
        }

        return View("Feature", FeatureModel<PaymentViewModel>(paymentService.ModuleCode, "Payment History", "Payment"));
    }

    [HttpGet("/Payment/Report")]
    public async Task<IActionResult> Report(CancellationToken cancellationToken)
    {
        if (!await CanAsync(paymentService, CrmPermissionActionConstants.Export, cancellationToken))
        {
            return Forbid();
        }

        return View("Feature", FeatureModel<PaymentViewModel>(paymentService.ModuleCode, "Payment Report", "Payment"));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        if (!await CanAsync(paymentService, CrmPermissionActionConstants.Delete, cancellationToken))
        {
            return Forbid();
        }

        var result = await paymentService.SoftDeleteAsync(id, cancellationToken);
        TempData[result.Succeeded ? "SuccessMessage" : "ErrorMessage"] = result.Succeeded
            ? result.Message
            : string.Join(" ", result.Errors);
        return RedirectToAction(nameof(Index));
    }
}
