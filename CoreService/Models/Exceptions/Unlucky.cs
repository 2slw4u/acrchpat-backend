using System.Net;

namespace CoreService.Models.Exceptions
{
    public class Unlucky : ExceptionToResponseProxy
    {
        public Unlucky() : base("You are unlucky", HttpStatusCode.InternalServerError) { }
        public Unlucky(string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError) : base(message) { }
    }
}
