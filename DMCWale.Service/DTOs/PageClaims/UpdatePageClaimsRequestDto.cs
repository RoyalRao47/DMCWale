namespace DMCWale.Service.DTOs.PageClaims;

public class UpdatePageClaimsRequestDto
{
    public Dictionary<string, HashSet<string>> RoleClaims { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}
