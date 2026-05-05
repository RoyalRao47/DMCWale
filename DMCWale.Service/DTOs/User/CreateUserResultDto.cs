namespace DMCWale.Service.DTOs.User;

public class CreateUserResultDto : OperationResultDto
{
    public int? UserId { get; set; }

    public string AgentSupplierCode { get; set; } = string.Empty;
}
