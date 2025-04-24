using System.Net;

namespace CoreService.Models.Exceptions
{
    public class InternalServerError : ExceptionToResponseProxy
    {
        public HttpStatusCode StatusCode { get; set; }
        
        public InternalServerError() : base("Внутренняя ошибка сервиса") {
            this.StatusCode = HttpStatusCode.InternalServerError;
        }
        public InternalServerError(string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError) : base(message) {
            this.StatusCode = statusCode;
        }
    }
}
