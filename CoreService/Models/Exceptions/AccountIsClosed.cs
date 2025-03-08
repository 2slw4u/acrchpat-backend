using System.Net;

namespace CoreService.Models.Exceptions
{
    public class AccountIsClosed : ExceptionToResponseProxy
    {
        public AccountIsClosed() : base("Искомый счет уже закрыт", HttpStatusCode.UnprocessableEntity) { }

        public AccountIsClosed(string message, HttpStatusCode statusCode = HttpStatusCode.UnprocessableEntity) : base(message) { }
    }
}
