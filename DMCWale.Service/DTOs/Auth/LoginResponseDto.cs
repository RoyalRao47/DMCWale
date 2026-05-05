namespace DMCWale.Service.DTOs.Auth;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string AgentSupplierCode { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public decimal WalletAmount { get; set; }
}
