using System.Net;

namespace CoreService.Models.Exceptions
{
    public class DataInContextNotFound : ExceptionToResponseProxy
    {
        public DataInContextNotFound() : base("Не смогли получить данные из HttpContext", HttpStatusCode.InternalServerError) { }

        public DataInContextNotFound(string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError) : base(message) { }
    }
}
