using DMCWale.Data.Models.Identity;

namespace DMCWale.Data.Models;

public class UserDetail
{
    public int Id { get; set; }

    public string AspNetUserId { get; set; } = string.Empty;

    public ApplicationUser AspNetUser { get; set; } = null!;

    public string Salutation { get; set; } = string.Empty;

    public string CountryCode { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string Address1 { get; set; } = string.Empty;

    public string Address2 { get; set; } = string.Empty;

    public string Signature { get; set; } = string.Empty;

    public string? ProfileImagePath { get; set; }

    public DateTime AddDate { get; set; }

    public DateTime? ModifyDate { get; set; }
}
