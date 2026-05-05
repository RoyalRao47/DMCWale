using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DMCWale.Admin.ViewModels.User;

public class EditUserViewModel
{
    public int Id { get; set; }

    public string AspNetUserId { get; set; } = string.Empty;

    [Required]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Mobile { get; set; } = string.Empty;

    [Display(Name = "Agent/Supplier Code")]
    public string AgentSupplierCode { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "New Password")]
    public string? Password { get; set; }

    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Confirm password must match password.")]
    [Display(Name = "Confirm New Password")]
    public string? ConfirmPassword { get; set; }

    [Required]
    [Display(Name = "Role")]
    public string RoleName { get; set; } = string.Empty;

    [Display(Name = "Is Active")]
    public bool IsActive { get; set; }

    [Display(Name = "Is Left")]
    public bool IsLeft { get; set; }

    public List<SelectListItem> Roles { get; set; } = new();
}
