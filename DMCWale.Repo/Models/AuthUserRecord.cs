using DMCWale.Data.Models;
using DMCWale.Data.Models.Identity;

namespace DMCWale.Repo.Models;

public class AuthUserRecord
{
    public ApplicationUser ApplicationUser { get; set; } = null!;

    public User? UserProfile { get; set; }

    public string RoleName { get; set; } = string.Empty;
}
