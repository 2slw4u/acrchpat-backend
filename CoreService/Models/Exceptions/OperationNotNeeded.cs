using System.Net;

namespace CoreService.Models.Exceptions
{
    public class OperationNotNeeded : ExceptionToResponseProxy
    {
        public OperationNotNeeded() : base("Данная операция - залог на будущее. Сейчас в ней нет необходимости", HttpStatusCode.NotImplemented) { }
        public OperationNotNeeded(string message, HttpStatusCode statusCode = HttpStatusCode.NotImplemented) : base(message) { }
    }
}
