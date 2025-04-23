using System.Net;

namespace PreferenceService.Exceptions;

public class HttpRequestFailedException : Exception
{
    public HttpStatusCode StatusCode { get; }

    public HttpRequestFailedException(HttpStatusCode statusCode, string message)
        : base(message)
    {
        StatusCode = statusCode;
    }
}