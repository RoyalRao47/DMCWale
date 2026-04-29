using Microsoft.AspNetCore.Mvc.Rendering;

namespace DMCWale.Admin.ViewModels.User;

public class UserListViewModel
{
    public string? RoleName { get; set; }

    public List<SelectListItem> Roles { get; set; } = new();

    public List<UserListItemViewModel> Users { get; set; } = new();
}
