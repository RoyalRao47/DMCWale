using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DMCWale.Data.Models.Identity;
using DMCWale.Repo.Interfaces;
using DMCWale.Service.DTOs.Auth;
using DMCWale.Service.Interfaces;
using DMCWale.Service.Models.Common;
using DMCWale.Service.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DMCWale.Service.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserRepository _userRepository;
    private readonly JwtOptions _jwtOptions;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        IUserRepository userRepository,
        IOptions<JwtOptions> jwtOptions)
    {
        _userManager = userManager;
        _userRepository = userRepository;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<LoginResponseDto> LoginAsync(
        LoginRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var agentSupplierCode = request.AgentSupplierCode?.Trim().ToUpperInvariant() ?? string.Empty;
        var email = request.Email?.Trim() ?? string.Empty;
        var password = request.Password ?? string.Empty;

        if (string.IsNullOrWhiteSpace(agentSupplierCode))
        {
            throw new AuthServiceException("Agent/Supplier Code is missing.");
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new AuthServiceException("Email/User Name is missing.");
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            throw new AuthServiceException("Password is missing.");
        }

        var normalizedEmailOrUserName = _userManager.NormalizeEmail(email);
        var userExists = await _userManager.Users
            .AsNoTracking()
            .AnyAsync(
                user =>
                    user.NormalizedEmail == normalizedEmailOrUserName ||
                    user.NormalizedUserName == normalizedEmailOrUserName,
                cancellationToken);

        if (!userExists)
        {
            throw new AuthServiceException("User does not exist.", 404);
        }

        var authUser = await _userRepository.GetByEmailAndAgentSupplierCodeAsync(
            normalizedEmailOrUserName,
            agentSupplierCode,
            cancellationToken);

        if (authUser is null)
        {
            throw new AuthServiceException("Agent/Supplier Code does not match.", 401);
        }

        var applicationUser = authUser.ApplicationUser;
        if (await _userManager.IsLockedOutAsync(applicationUser))
        {
            throw new AuthServiceException("User inactive/locked.", 403);
        }

        if (authUser.UserProfile is null ||
            !authUser.UserProfile.IsActive ||
            authUser.UserProfile.IsLeft)
        {
            throw new AuthServiceException("User inactive/locked.", 403);
        }

        if (!await _userManager.CheckPasswordAsync(applicationUser, password))
        {
            throw new AuthServiceException("Invalid password.", 401);
        }

        var fullName = string.Join(
            " ",
            new[] { authUser.UserProfile.FirstName, authUser.UserProfile.LastName }
                .Where(value => !string.IsNullOrWhiteSpace(value)));

        return new LoginResponseDto
        {
            Token = GenerateToken(applicationUser, authUser.RoleName, fullName, agentSupplierCode),
            UserId = applicationUser.Id,
            FullName = fullName,
            Email = applicationUser.Email ?? email,
            AgentSupplierCode = agentSupplierCode,
            Role = authUser.RoleName,
            WalletAmount = 0m
        };
    }

    private string GenerateToken(
        ApplicationUser user,
        string roleName,
        string fullName,
        string agentSupplierCode)
    {
        if (string.IsNullOrWhiteSpace(_jwtOptions.Secret))
        {
            throw new InvalidOperationException("JWT secret is not configured.");
        }

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Name, fullName),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName ?? user.Email ?? string.Empty),
            new(ClaimTypes.Role, roleName),
            new("AgentSupplierCode", agentSupplierCode)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiryMinutes = _jwtOptions.ExpiryMinutes <= 0 ? 120 : _jwtOptions.ExpiryMinutes;

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
