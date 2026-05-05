using Microsoft.AspNetCore.Identity;

namespace DMCWale.Data.Models.Identity;

public class ApplicationUser : IdentityUser
{
    public string? AgentSupplierCode { get; set; }

    public User? UserProfile { get; set; }

    public UserDetail? UserDetail { get; set; }
}
