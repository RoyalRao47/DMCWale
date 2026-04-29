using System.Security.Claims;
using DMCWale.Data.Constants;
using DMCWale.Data.Models.Identity;
using DMCWale.Service.DTOs.PageClaims;
using DMCWale.Service.Interfaces;
using DMCWale.Service.Models.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DMCWale.Service.Services;

public class PageClaimService : IPageClaimService
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ILogger<PageClaimService> _logger;

    public PageClaimService(RoleManager<ApplicationRole> roleManager, ILogger<PageClaimService> logger)
    {
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task<PageClaimMatrixDto> GetPageClaimsAsync(CancellationToken cancellationToken = default)
    {
        var roles = await _roleManager.Roles
            .AsNoTracking()
            .OrderBy(role => role.Name)
            .ToListAsync(cancellationToken);

        var roleDtos = new List<PageClaimRoleDto>();

        foreach (var role in roles)
        {
            var claims = await _roleManager.GetClaimsAsync(role);
            roleDtos.Add(new PageClaimRoleDto
            {
                RoleId = role.Id,
                RoleName = role.Name ?? string.Empty,
                ClaimValues = claims
                    .Where(claim => claim.Type == PagePermissionConstants.ClaimType)
                    .Select(claim => claim.Value)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase)
            });
        }

        return new PageClaimMatrixDto
        {
            Roles = roleDtos,
            Permissions = PagePermissionConstants.All
                .Select(permission => new PageClaimPermissionDto
                {
                    Module = permission.Module,
                    Name = permission.Name,
                    ClaimValue = permission.ClaimValue
                })
                .ToList()
        };
    }

    public async Task<ServiceResult> UpdatePageClaimsAsync(
        UpdatePageClaimsRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var validClaims = PagePermissionConstants.All
            .Select(permission => permission.ClaimValue)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var roles = await _roleManager.Roles
            .OrderBy(role => role.Name)
            .ToListAsync(cancellationToken);

        foreach (var role in roles)
        {
            var requestedClaims = request.RoleClaims.TryGetValue(role.Id, out var claims)
                ? claims.Where(validClaims.Contains).ToHashSet(StringComparer.OrdinalIgnoreCase)
                : new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var currentClaims = await _roleManager.GetClaimsAsync(role);
            var currentPageClaims = currentClaims
                .Where(claim => claim.Type == PagePermissionConstants.ClaimType)
                .ToList();

            foreach (var currentClaim in currentPageClaims.Where(claim => !requestedClaims.Contains(claim.Value)))
            {
                var removeResult = await _roleManager.RemoveClaimAsync(role, currentClaim);
                if (!removeResult.Succeeded)
                {
                    _logger.LogWarning(
                        "Could not remove page claim {ClaimValue} from role {RoleName}: {Errors}",
                        currentClaim.Value,
                        role.Name,
                        string.Join(", ", removeResult.Errors.Select(error => error.Description)));

                    return ServiceResult.Failure(
                        "Page permissions could not be updated.",
                        removeResult.Errors.Select(error => error.Description));
                }
            }

            var existingValues = currentPageClaims
                .Select(claim => claim.Value)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var requestedClaim in requestedClaims.Where(claim => !existingValues.Contains(claim)))
            {
                var addResult = await _roleManager.AddClaimAsync(
                    role,
                    new Claim(PagePermissionConstants.ClaimType, requestedClaim));

                if (!addResult.Succeeded)
                {
                    _logger.LogWarning(
                        "Could not add page claim {ClaimValue} to role {RoleName}: {Errors}",
                        requestedClaim,
                        role.Name,
                        string.Join(", ", addResult.Errors.Select(error => error.Description)));

                    return ServiceResult.Failure(
                        "Page permissions could not be updated.",
                        addResult.Errors.Select(error => error.Description));
                }
            }
        }

        return ServiceResult.Success("Page permissions updated successfully.");
    }
}
