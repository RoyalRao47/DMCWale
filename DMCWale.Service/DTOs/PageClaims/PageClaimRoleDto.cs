namespace DMCWale.Service.DTOs.PageClaims;

public class PageClaimRoleDto
{
    public string RoleId { get; set; } = string.Empty;

    public string RoleName { get; set; } = string.Empty;

    public HashSet<string> ClaimValues { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}
