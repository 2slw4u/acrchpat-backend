using System.Net;

namespace CoreService.Models.Exceptions
{
    public class TransactionNotFound : ExceptionToResponseProxy
    {
        public TransactionNotFound() : base("Искомая транзакция не найдена", HttpStatusCode.NotFound) { }
        public TransactionNotFound(string message, HttpStatusCode statusCode = HttpStatusCode.NotFound) : base(message) { }
    }
}
