namespace CoreService.Models.Exceptions
{
    public class AccountIsClosed : ExceptionToResponseProxy
    {
        public AccountIsClosed() : base("Искомый счет уже закрыт", false)
        {
            this.StatusCode = System.Net.HttpStatusCode.UnprocessableEntity;
        }
    }
}
