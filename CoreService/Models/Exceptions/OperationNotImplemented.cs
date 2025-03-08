using System.Net;

namespace CoreService.Models.Exceptions
{
    public class OperationNotImplemented : ExceptionToResponseProxy
    {
        public OperationNotImplemented() : base("Данная операция ещё не внедрена", HttpStatusCode.NotImplemented) { }
    }
}
