using System.Net;

namespace CoreService.Models.Exceptions
{
    public class UserServiceError : ExceptionToResponseProxy
    {
        public UserServiceError() : base("Некорректный запрос к сервису пользователей", HttpStatusCode.InternalServerError) { }
        public UserServiceError(string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError) : base(message) { }
    }
}
