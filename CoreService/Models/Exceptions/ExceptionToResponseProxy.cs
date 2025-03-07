using System.Net;

namespace CoreService.Models.Exceptions
{
    public abstract class ExceptionToResponseProxy : SystemException
    {
        public HttpStatusCode StatusCode { get; set; }
        
        public ExceptionToResponseProxy() : base("Внутренняя ошибка сервиса") {
            this.StatusCode = HttpStatusCode.InternalServerError;
        }
        public ExceptionToResponseProxy(string message, bool useDefaultCode = true) : base(message) {
            if (!useDefaultCode)
            {
                this.StatusCode = HttpStatusCode.InternalServerError;
            }
        }
    }
}
