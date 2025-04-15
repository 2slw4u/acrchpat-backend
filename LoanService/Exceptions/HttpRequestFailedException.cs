using System.Net;

namespace LoanService.Exceptions;

public class HttpRequestFailedException : Exception
{
    public HttpStatusCode StatusCode { get; }

    public HttpRequestFailedException(HttpStatusCode statusCode, string message)
        : base(message)
    {
        StatusCode = statusCode;
    }
}