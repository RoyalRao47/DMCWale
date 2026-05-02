using DMCWale.Admin.ViewModels.Booking;
using DMCWale.Data.Constants;
using DMCWale.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DMCWale.Admin.Controllers;

public class BookingController(IBookingService bookingService) : CrmModuleControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        if (!await CanAsync(bookingService, CrmPermissionActionConstants.View, cancellationToken))
        {
            return Forbid();
        }

        return View(Map<BookingViewModel>(await bookingService.GetListAsync(cancellationToken)));
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        if (!await CanAsync(bookingService, CrmPermissionActionConstants.View, cancellationToken))
        {
            return Forbid();
        }

        return View(Map<BookingViewModel>(await bookingService.GetDetailsAsync(id, cancellationToken)));
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        if (!await CanAsync(bookingService, CrmPermissionActionConstants.Add, cancellationToken))
        {
            return Forbid();
        }

        return View(Map<BookingViewModel>(await bookingService.GetFormAsync(cancellationToken: cancellationToken)));
    }

    [HttpGet("/Booking/CreateFromQuotation/{quotationId:int}")]
    public async Task<IActionResult> CreateFromQuotation(int quotationId, CancellationToken cancellationToken)
    {
        if (!await CanAsync(bookingService, CrmPermissionActionConstants.Add, cancellationToken))
        {
            return Forbid();
        }

        return View("Feature", FeatureModel<BookingViewModel>(bookingService.ModuleCode, "Create Booking From Quotation", "Booking"));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BookingViewModel model, CancellationToken cancellationToken)
    {
        if (!await CanAsync(bookingService, CrmPermissionActionConstants.Add, cancellationToken))
        {
            return Forbid();
        }

        var result = await bookingService.SaveAsync(ToSaveRequest(model), cancellationToken);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        AddErrors(result.Errors);
        return View(await RebuildFormAsync<BookingViewModel>(bookingService, model, cancellationToken: cancellationToken));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        if (!await CanAsync(bookingService, CrmPermissionActionConstants.Edit, cancellationToken))
        {
            return Forbid();
        }

        var model = Map<BookingViewModel>(await bookingService.GetFormAsync(id, cancellationToken));
        model.Rows = [new() { Id = id }];
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BookingViewModel model, CancellationToken cancellationToken)
    {
        if (!await CanAsync(bookingService, CrmPermissionActionConstants.Edit, cancellationToken))
        {
            return Forbid();
        }

        var request = ToSaveRequest(model);
        request.Id = id;
        var result = await bookingService.SaveAsync(request, cancellationToken);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        AddErrors(result.Errors);
        return View(await RebuildFormAsync<BookingViewModel>(bookingService, model, id, cancellationToken));
    }

    [HttpGet("/Booking/Status/{id:int}")]
    public async Task<IActionResult> Status(int id, CancellationToken cancellationToken)
    {
        if (!await CanAsync(bookingService, CrmPermissionActionConstants.Edit, cancellationToken))
        {
            return Forbid();
        }

        return View("Feature", FeatureModel<BookingViewModel>(bookingService.ModuleCode, "Booking Status", "Booking"));
    }

    [HttpGet("/Booking/Reassign/{id:int}")]
    public async Task<IActionResult> Reassign(int id, CancellationToken cancellationToken)
    {
        if (!await CanAsync(bookingService, CrmPermissionActionConstants.Reassign, cancellationToken))
        {
            return Forbid();
        }

        return View("Feature", FeatureModel<BookingViewModel>(bookingService.ModuleCode, "Reassign Booking", "Booking"));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        if (!await CanAsync(bookingService, CrmPermissionActionConstants.Delete, cancellationToken))
        {
            return Forbid();
        }

        var result = await bookingService.SoftDeleteAsync(id, cancellationToken);
        TempData[result.Succeeded ? "SuccessMessage" : "ErrorMessage"] = result.Succeeded
            ? result.Message
            : string.Join(" ", result.Errors);
        return RedirectToAction(nameof(Index));
    }
}
