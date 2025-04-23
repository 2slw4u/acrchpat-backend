using System.Net;

namespace CoreService.Models.Exceptions
{
    public class UniRateError : ExceptionToResponseProxy
    {
        public UniRateError() : base("Некорректный запрос к сервису UniRate", HttpStatusCode.InternalServerError) { }
        public UniRateError(string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError) : base(message) { }
    }
}
