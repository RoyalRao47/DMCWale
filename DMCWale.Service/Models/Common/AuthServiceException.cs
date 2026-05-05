namespace DMCWale.Service.Models.Common;

public class AuthServiceException : Exception
{
    public AuthServiceException(string message, int statusCode = 400)
        : base(message)
    {
        StatusCode = statusCode;
    }

    public int StatusCode { get; }
}
