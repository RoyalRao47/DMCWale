namespace DMCWale.Service.DTOs.User;

public class CreateUserRequestDto
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Mobile { get; set; } = string.Empty;

    public string AgentSupplierCode { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string RoleName { get; set; } = string.Empty;
}
