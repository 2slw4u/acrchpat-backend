using System.Net;

namespace CoreService.Models.Exceptions
{
    public abstract class ExceptionToResponseProxy : SystemException
    {
        public HttpStatusCode StatusCode { get; set; }
        
        public ExceptionToResponseProxy() : base("Внутренняя ошибка сервиса") {
            this.StatusCode = HttpStatusCode.InternalServerError;
        }
        public ExceptionToResponseProxy(string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError) : base(message) {
            this.StatusCode = statusCode;
        }
    }
}
