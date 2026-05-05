namespace DMCWale.Service.DTOs.User;

public class OperationResultDto
{
    public bool Succeeded { get; set; }

    public string Message { get; set; } = string.Empty;

    public List<string> Errors { get; set; } = [];

    public static OperationResultDto Success(string message)
    {
        return new OperationResultDto
        {
            Succeeded = true,
            Message = message
        };
    }

    public static OperationResultDto Failure(string message, IEnumerable<string>? errors = null)
    {
        return new OperationResultDto
        {
            Succeeded = false,
            Message = message,
            Errors = errors?.ToList() ?? [message]
        };
    }
}
