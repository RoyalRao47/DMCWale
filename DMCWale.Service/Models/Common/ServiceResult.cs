namespace DMCWale.Service.Models.Common;

public class ServiceResult
{
    public bool Succeeded { get; set; }

    public string Message { get; set; } = string.Empty;

    public List<string> Errors { get; set; } = [];

    public string RedirectUrl { get; set; } = string.Empty;

    public static ServiceResult Success(string message = "", string redirectUrl = "")
    {
        return new ServiceResult
        {
            Succeeded = true,
            Message = message,
            RedirectUrl = redirectUrl
        };
    }

    public static ServiceResult Failure(string message, IEnumerable<string>? errors = null)
    {
        return new ServiceResult
        {
            Succeeded = false,
            Message = message,
            Errors = errors?.ToList() ?? [message]
        };
    }
}
