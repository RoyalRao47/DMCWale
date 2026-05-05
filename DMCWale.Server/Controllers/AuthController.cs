using DMCWale.Service.DTOs.Auth;
using DMCWale.Service.Interfaces;
using DMCWale.Service.Models.Common;
using Microsoft.AspNetCore.Mvc;

namespace DMCWale.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(
        LoginRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _authService.LoginAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (AuthServiceException exception)
        {
            return StatusCode(exception.StatusCode, new { message = exception.Message });
        }
    }
}
