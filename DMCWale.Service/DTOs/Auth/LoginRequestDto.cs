namespace DMCWale.Service.DTOs.Auth;

public class LoginRequestDto
{
    public string AgentSupplierCode { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}
