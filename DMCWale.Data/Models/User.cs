using DMCWale.Data.Models.Identity;

namespace DMCWale.Data.Models;

public class User
{
    public int Id { get; set; }

    public string AspNetUserId { get; set; } = string.Empty;

    public ApplicationUser AspNetUser { get; set; } = null!;

    public string Username { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Mobile { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public bool IsLeft { get; set; }

    public DateTime AddDate { get; set; }

    public DateTime? ModifyDate { get; set; }
}
