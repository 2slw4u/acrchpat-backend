using System.Net;

namespace CoreService.Models.Exceptions
{
    public class AccountIsClosed : ExceptionToResponseProxy
    {
        public AccountIsClosed() : base("Искомый счет уже закрыт", HttpStatusCode.UnprocessableEntity) { }
    }
}
