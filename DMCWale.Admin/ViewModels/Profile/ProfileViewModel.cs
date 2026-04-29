using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DMCWale.Admin.ViewModels.Profile;

public class ProfileViewModel
{
    public ProfileDetailsViewModel Details { get; set; } = new();

    public ChangePasswordViewModel Password { get; set; } = new();
}

public class ProfileDetailsViewModel
{
    [Required]
    [Display(Name = "First Name")]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Last Name")]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Mobile")]
    [StringLength(30)]
    [RegularExpression(@"^[0-9\s+\-()]{7,30}$", ErrorMessage = "Enter a valid mobile number.")]
    public string Mobile { get; set; } = string.Empty;

    [Display(Name = "Title")]
    [StringLength(20)]
    public string Salutation { get; set; } = string.Empty;

    [Display(Name = "Code")]
    [StringLength(10)]
    [RegularExpression(@"^\+?[0-9]{1,4}$", ErrorMessage = "Enter a valid code, for example +91.")]
    public string CountryCode { get; set; } = string.Empty;

    [StringLength(100)]
    public string City { get; set; } = string.Empty;

    [Display(Name = "Address 1")]
    [StringLength(250)]
    public string Address1 { get; set; } = string.Empty;

    [Display(Name = "Address 2")]
    [StringLength(250)]
    public string Address2 { get; set; } = string.Empty;

    [StringLength(4000)]
    public string Signature { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? ProfileImagePath { get; set; }

    public IFormFile? ProfileImage { get; set; }

    public List<SelectListItem> Salutations { get; set; } = [];
}

public class ChangePasswordViewModel
{
    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Old Password")]
    public string OldPassword { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6)]
    [Display(Name = "New Password")]
    public string NewPassword { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(NewPassword), ErrorMessage = "Confirm password must match new password.")]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
