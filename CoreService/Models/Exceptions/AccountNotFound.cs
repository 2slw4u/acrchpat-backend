using System.Net;

namespace CoreService.Models.Exceptions
{
    public class AccountNotFound : ExceptionToResponseProxy
    {
        public AccountNotFound() : base("Искомый счет не найден", HttpStatusCode.NotFound) { }
        public AccountNotFound(string message, HttpStatusCode statusCode = HttpStatusCode.NotFound) : base(message) { }
    }
}
