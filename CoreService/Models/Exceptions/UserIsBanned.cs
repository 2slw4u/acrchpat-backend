using System.Net;

namespace CoreService.Models.Exceptions
{
    public class UserIsBanned : ExceptionToResponseProxy
    {
        public UserIsBanned() : base("Пользователь забанен", HttpStatusCode.Forbidden) { }
        public UserIsBanned(string message, HttpStatusCode statusCode = HttpStatusCode.Forbidden) : base(message) { }
    }
}
