using System.Net;

namespace CoreService.Models.Exceptions
{
    public class UserDoesntOwnTheAccount : ExceptionToResponseProxy
    {
        public UserDoesntOwnTheAccount() : base("Пользователь не владеет этим счетом", HttpStatusCode.Forbidden) { }
        public UserDoesntOwnTheAccount(string message, HttpStatusCode statusCode = HttpStatusCode.Forbidden) : base(message) { }
    }
}
