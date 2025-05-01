using System.Net;

namespace CoreService.Models.Exceptions
{
    public class RequestIsAlreadyProcessing : ExceptionToResponseProxy
    {
        public RequestIsAlreadyProcessing() : base("Запрос уже находится в обработке сервиса, подождите", HttpStatusCode.Conflict) { }

        public RequestIsAlreadyProcessing(string message, HttpStatusCode statusCode = HttpStatusCode.Conflict) : base(message) { }
    }
}
