using DMCWale.Admin.ViewModels.InventorySearch;
using DMCWale.Data.Constants;
using DMCWale.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DMCWale.Admin.Controllers;

public class InventorySearchController(IInventorySearchService inventorySearchService) : CrmModuleControllerBase
{
    [HttpGet]
    public Task<IActionResult> Hotel(CancellationToken cancellationToken)
    {
        return SearchPageAsync("Hotel Inventory Search", cancellationToken);
    }

    [HttpGet]
    public Task<IActionResult> Sightseeing(CancellationToken cancellationToken)
    {
        return SearchPageAsync("Sightseeing Inventory Search", cancellationToken);
    }

    [HttpGet]
    public Task<IActionResult> Transfer(CancellationToken cancellationToken)
    {
        return SearchPageAsync("Transfer Inventory Search", cancellationToken);
    }

    private async Task<IActionResult> SearchPageAsync(string title, CancellationToken cancellationToken)
    {
        if (!await CanAsync(inventorySearchService, CrmPermissionActionConstants.View, cancellationToken))
        {
            return Forbid();
        }

        return View(ControllerContext.ActionDescriptor.ActionName, FeatureModel<InventorySearchViewModel>(inventorySearchService.ModuleCode, title, "InventorySearch"));
    }
}
