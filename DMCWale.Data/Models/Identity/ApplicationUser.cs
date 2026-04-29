using Microsoft.AspNetCore.Identity;

namespace DMCWale.Data.Models.Identity;

public class ApplicationUser : IdentityUser
{
    public User? UserProfile { get; set; }

    public UserDetail? UserDetail { get; set; }
}
