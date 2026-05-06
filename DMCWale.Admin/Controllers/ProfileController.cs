using System.Security.Claims;
using DMCWale.Admin.Filters;
using DMCWale.Admin.ViewModels.Profile;
using DMCWale.Data.Constants;
using DMCWale.Service.DTOs.Profile;
using DMCWale.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DMCWale.Admin.Controllers;

[Authorize]
[PagePermissionAuthorize(PagePermissionConstants.ProfileView)]
public class ProfileController : Controller
{
    private static readonly HashSet<string> AllowedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".webp"
    };

    private const long MaxProfileImageBytes = 2 * 1024 * 1024;

    private readonly IProfileService _profileService;
    private readonly IWebHostEnvironment _environment;

    public ProfileController(IProfileService profileService, IWebHostEnvironment environment)
    {
        _profileService = profileService;
        _environment = environment;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        return View(await BuildProfileViewModelAsync(cancellationToken: cancellationToken));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(
        [Bind(Prefix = nameof(ProfileViewModel.Details))] ProfileDetailsViewModel details,
        CancellationToken cancellationToken)
    {
        ClearModelStateForPrefix(nameof(ProfileViewModel.Password));

        if (!ModelState.IsValid)
        {
            return View(nameof(Index), await BuildProfileViewModelAsync(details, new ChangePasswordViewModel(), cancellationToken));
        }

        var aspNetUserId = GetCurrentUserId();
        if (aspNetUserId is null)
        {
            return Challenge();
        }

        var result = await _profileService.UpdateProfileAsync(
            aspNetUserId,
            new UpdateProfileRequestDto
            {
                FirstName = details.FirstName,
                LastName = details.LastName,
                Mobile = details.Mobile,
                Email = details.Email,
                Salutation = details.Salutation,
                CountryCode = details.CountryCode,
                City = details.City,
                Address1 = details.Address1,
                Address2 = details.Address2,
                Signature = details.Signature
            },
            cancellationToken);

        if (!result.Succeeded)
        {
            AddServiceErrors(result.Errors);
            return View(nameof(Index), await BuildProfileViewModelAsync(details, new ChangePasswordViewModel(), cancellationToken));
        }

        TempData["SuccessMessage"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(
        [Bind(Prefix = nameof(ProfileViewModel.Password))] ChangePasswordViewModel password,
        CancellationToken cancellationToken)
    {
        ClearModelStateForPrefix(nameof(ProfileViewModel.Details));

        if (!ModelState.IsValid)
        {
            return View(nameof(Index), await BuildProfileViewModelAsync(passwordModel: password, cancellationToken: cancellationToken));
        }

        var aspNetUserId = GetCurrentUserId();
        if (aspNetUserId is null)
        {
            return Challenge();
        }

        var result = await _profileService.ChangePasswordAsync(
            aspNetUserId,
            new ChangePasswordRequestDto
            {
                OldPassword = password.OldPassword,
                NewPassword = password.NewPassword
            });

        if (!result.Succeeded)
        {
            AddServiceErrors(result.Errors, $"{nameof(ProfileViewModel.Password)}.{nameof(ChangePasswordViewModel.OldPassword)}");
            return View(nameof(Index), await BuildProfileViewModelAsync(passwordModel: password, cancellationToken: cancellationToken));
        }

        TempData["SuccessMessage"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadPhoto(
        [FromForm(Name = "Details.ProfileImage")] IFormFile? profileImage,
        CancellationToken cancellationToken)
    {
        var aspNetUserId = GetCurrentUserId();
        if (aspNetUserId is null)
        {
            return Challenge();
        }

        if (profileImage is null || profileImage.Length == 0)
        {
            ModelState.AddModelError($"{nameof(ProfileViewModel.Details)}.{nameof(ProfileDetailsViewModel.ProfileImage)}", "Choose a profile image.");
            return View(nameof(Index), await BuildProfileViewModelAsync(cancellationToken: cancellationToken));
        }

        var extension = Path.GetExtension(profileImage.FileName);
        if (!AllowedImageExtensions.Contains(extension))
        {
            ModelState.AddModelError($"{nameof(ProfileViewModel.Details)}.{nameof(ProfileDetailsViewModel.ProfileImage)}", "Use a JPG, PNG, or WEBP image.");
            return View(nameof(Index), await BuildProfileViewModelAsync(cancellationToken: cancellationToken));
        }

        if (profileImage.Length > MaxProfileImageBytes)
        {
            ModelState.AddModelError($"{nameof(ProfileViewModel.Details)}.{nameof(ProfileDetailsViewModel.ProfileImage)}", "Profile image must be 2 MB or smaller.");
            return View(nameof(Index), await BuildProfileViewModelAsync(cancellationToken: cancellationToken));
        }

        var uploadDirectory = Path.Combine(_environment.WebRootPath, "uploads", "profiles");
        Directory.CreateDirectory(uploadDirectory);

        var fileName = $"{aspNetUserId}{extension.ToLowerInvariant()}";
        var filePath = Path.Combine(uploadDirectory, fileName);

        await using (var stream = System.IO.File.Create(filePath))
        {
            await profileImage.CopyToAsync(stream, cancellationToken);
        }

        var result = await _profileService.UpdateProfileImagePathAsync(
            aspNetUserId,
            $"/uploads/profiles/{fileName}",
            cancellationToken);

        if (!result.Succeeded)
        {
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            AddServiceErrors(result.Errors);
            return View(nameof(Index), await BuildProfileViewModelAsync(cancellationToken: cancellationToken));
        }

        TempData["SuccessMessage"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    private async Task<ProfileViewModel> BuildProfileViewModelAsync(
        ProfileDetailsViewModel? detailsModel = null,
        ChangePasswordViewModel? passwordModel = null,
        CancellationToken cancellationToken = default)
    {
        var aspNetUserId = GetCurrentUserId();
        var profile = aspNetUserId is null
            ? null
            : await _profileService.GetProfileAsync(aspNetUserId, cancellationToken);

        return new ProfileViewModel
        {
            Details = detailsModel is null
                ? BuildDetailsModel(profile)
                : AttachProfileHeader(detailsModel, profile),
            Password = passwordModel ?? new ChangePasswordViewModel()
        };
    }

    private static ProfileDetailsViewModel BuildDetailsModel(ProfileDetailsDto? profile)
    {
        return AttachProfileHeader(
            new ProfileDetailsViewModel
            {
                FirstName = profile?.FirstName ?? string.Empty,
                LastName = profile?.LastName ?? string.Empty,
                Mobile = profile?.Mobile ?? string.Empty,
                Salutation = profile?.Salutation ?? "Mr.",
                CountryCode = profile?.CountryCode ?? string.Empty,
                City = profile?.City ?? string.Empty,
                Address1 = profile?.Address1 ?? string.Empty,
                Address2 = profile?.Address2 ?? string.Empty,
                Signature = profile?.Signature ?? string.Empty
            },
            profile);
    }

    private static ProfileDetailsViewModel AttachProfileHeader(
        ProfileDetailsViewModel model,
        ProfileDetailsDto? profile)
    {
        model.Username = profile?.Username ?? string.Empty;
        model.Email = profile?.Email ?? string.Empty;
        model.ProfileImagePath = profile?.ProfileImagePath;
        model.Salutations = GetSalutations(model.Salutation);
        return model;
    }

    private string? GetCurrentUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    private static List<SelectListItem> GetSalutations(string? selectedValue)
    {
        var salutations = new[] { "Mr.", "Mrs.", "Ms.", "Dr." };

        return salutations.Select(salutation => new SelectListItem
        {
            Text = salutation,
            Value = salutation,
            Selected = salutation == selectedValue
        }).ToList();
    }

    private void AddServiceErrors(IEnumerable<string> errors, string key = "")
    {
        foreach (var error in errors)
        {
            ModelState.AddModelError(key, error);
        }
    }

    private void ClearModelStateForPrefix(string prefix)
    {
        foreach (var key in ModelState.Keys.Where(key => key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)).ToList())
        {
            ModelState.Remove(key);
        }
    }
}
