using System.Net;

namespace CoreService.Models.Exceptions
{
    public class NotEnoughMoney : ExceptionToResponseProxy
    {
        public NotEnoughMoney() : base("На счете недостаточно денег для списания", HttpStatusCode.UnprocessableEntity) { }
        public NotEnoughMoney(string message, HttpStatusCode statusCode = HttpStatusCode.UnprocessableEntity) : base(message) { }
    }
}
