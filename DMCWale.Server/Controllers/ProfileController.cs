using System.Security.Claims;
using DMCWale.Service.DTOs.Profile;
using DMCWale.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMCWale.Server.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;

    public ProfileController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    [HttpGet]
    public async Task<ActionResult<ProfileDetailsDto>> Get(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(new { message = "User session is invalid." });
        }

        var profile = await _profileService.GetProfileAsync(userId, cancellationToken);
        return profile is null
            ? NotFound(new { message = "Profile was not found." })
            : Ok(profile);
    }

    [HttpPut]
    public async Task<IActionResult> Update(
        UpdateProfileRequestDto request,
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(new { message = "User session is invalid." });
        }

        var result = await _profileService.UpdateProfileAsync(userId, request, cancellationToken);
        if (!result.Succeeded)
        {
            return BadRequest(new { message = result.Message, errors = result.Errors });
        }

        var profile = await _profileService.GetProfileAsync(userId, cancellationToken);
        return Ok(new { message = result.Message, profile });
    }
}
