namespace DMCWale.Service.DTOs.PageClaims;

public class PageClaimMatrixDto
{
    public List<PageClaimRoleDto> Roles { get; set; } = new();

    public List<PageClaimPermissionDto> Permissions { get; set; } = new();
}
