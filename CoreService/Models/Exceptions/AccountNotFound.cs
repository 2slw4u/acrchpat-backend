using System.Net;

namespace CoreService.Models.Exceptions
{
    public class AccountNotFound : ExceptionToResponseProxy
    {
        public AccountNotFound() : base("Искомый счет не найден", HttpStatusCode.NotFound) { }
    }
}
