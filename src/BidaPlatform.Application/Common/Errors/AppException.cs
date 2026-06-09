using System.Net;

namespace BidaPlatform.Application.Common.Errors;

public class AppException : Exception
{
    public string ErrorCode { get; }
    public HttpStatusCode StatusCode { get; }

    public AppException(
        string errorCode,
        string message,
        HttpStatusCode statusCode = HttpStatusCode.BadRequest,
        Exception? innerException = null)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
    }
}
