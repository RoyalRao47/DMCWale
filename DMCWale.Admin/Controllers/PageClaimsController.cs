using DMCWale.Admin.ViewModels.PageClaims;
using DMCWale.Data.Constants;
using DMCWale.Service.DTOs.PageClaims;
using DMCWale.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMCWale.Admin.Controllers;

[Authorize(Roles = RoleConstants.Admin)]
public class PageClaimsController : Controller
{
    private const string ClaimPostSeparator = "||";
    private readonly IPageClaimService _pageClaimService;

    public PageClaimsController(IPageClaimService pageClaimService)
    {
        _pageClaimService = pageClaimService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        return View(await BuildModelAsync(cancellationToken));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(PageClaimEditViewModel model, CancellationToken cancellationToken)
    {
        var request = new UpdatePageClaimsRequestDto();

        foreach (var selectedClaim in model.SelectedClaims)
        {
            var parts = selectedClaim.Split(ClaimPostSeparator, StringSplitOptions.None);
            if (parts.Length != 2)
            {
                continue;
            }

            if (!request.RoleClaims.TryGetValue(parts[0], out var roleClaims))
            {
                roleClaims = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                request.RoleClaims[parts[0]] = roleClaims;
            }

            roleClaims.Add(parts[1]);
        }

        var result = await _pageClaimService.UpdatePageClaimsAsync(request, cancellationToken);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }

        return View(await BuildModelAsync(cancellationToken));
    }

    private async Task<PageClaimEditViewModel> BuildModelAsync(CancellationToken cancellationToken)
    {
        var matrix = await _pageClaimService.GetPageClaimsAsync(cancellationToken);

        return new PageClaimEditViewModel
        {
            Roles = matrix.Roles,
            Permissions = matrix.Permissions,
            SelectedClaims = matrix.Roles
                .SelectMany(role => role.ClaimValues.Select(claim => $"{role.RoleId}{ClaimPostSeparator}{claim}"))
                .ToList()
        };
    }
}
