using DMCWale.Service.DTOs.Role;

namespace DMCWale.Service.DTOs.User;

public class UserListResultDto
{
    public List<UserListItemDto> Users { get; set; } = new();

    public List<RoleDto> Roles { get; set; } = new();

    public string? SelectedRole { get; set; }
}
