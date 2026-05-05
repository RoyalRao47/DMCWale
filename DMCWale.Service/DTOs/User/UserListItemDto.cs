namespace DMCWale.Service.DTOs.User;

public class UserListItemDto
{
    public int Id { get; set; }

    public string AspNetUserId { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Mobile { get; set; } = string.Empty;

    public string AgentSupplierCode { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public bool IsLeft { get; set; }

    public string RoleName { get; set; } = string.Empty;

    public DateTime AddDate { get; set; }

    public DateTime? ModifyDate { get; set; }
}
