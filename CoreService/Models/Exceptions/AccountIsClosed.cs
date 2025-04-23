using System.Net;

namespace CoreService.Models.Exceptions
{
    public class TransferToMasterAccount : ExceptionToResponseProxy
    {
        public TransferToMasterAccount() : base("Искомый счет уже закрыт", HttpStatusCode.UnprocessableEntity) { }

        public TransferToMasterAccount(string message, HttpStatusCode statusCode = HttpStatusCode.UnprocessableEntity) : base(message) { }
    }
}
