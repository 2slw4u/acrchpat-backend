using System.Net;

namespace CoreService.Models.Exceptions
{
    public class UserDoesntOwnRequiredRole : ExceptionToResponseProxy
    {
        public UserDoesntOwnRequiredRole() : base("Пользователь не обладает достаточными правами доступа для совершения операции", HttpStatusCode.Forbidden) { }
        public UserDoesntOwnRequiredRole(string message, HttpStatusCode statusCode = HttpStatusCode.Forbidden) : base(message) { }
    }
}
