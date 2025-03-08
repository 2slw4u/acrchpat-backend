using System.Net;

namespace CoreService.Models.Exceptions
{
    public class NotEnoughMoney : ExceptionToResponseProxy
    {
        public NotEnoughMoney() : base("На счете недостаточно денег для списания", HttpStatusCode.UnprocessableEntity) { }
    }
}
