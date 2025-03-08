using System.Net;

namespace CoreService.Models.Exceptions
{
    public class OperationNotImplemented : ExceptionToResponseProxy
    {
        public OperationNotImplemented() : base("Данная операция ещё не внедрена", HttpStatusCode.NotImplemented) { }
        public OperationNotImplemented(string message, HttpStatusCode statusCode = HttpStatusCode.NotImplemented) : base(message) { }
    }
}
