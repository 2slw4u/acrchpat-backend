using System.Net;

namespace CoreService.Models.Exceptions
{
    public class AccountBalanceNotZero : ExceptionToResponseProxy
    {
        public AccountBalanceNotZero() : base("Баланс счета не равен нулю. Закрыть его не получится", HttpStatusCode.UnprocessableEntity) { }

        public AccountBalanceNotZero(string message, HttpStatusCode statusCode = HttpStatusCode.UnprocessableEntity) : base(message) { }
    }
}
