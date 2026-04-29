using DMCWale.Service.DTOs.PageClaims;

namespace DMCWale.Admin.ViewModels.PageClaims;

public class PageClaimEditViewModel
{
    public List<PageClaimRoleDto> Roles { get; set; } = new();

    public List<PageClaimPermissionDto> Permissions { get; set; } = new();

    public List<string> SelectedClaims { get; set; } = new();
}
