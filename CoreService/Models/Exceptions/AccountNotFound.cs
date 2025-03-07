namespace CoreService.Models.Exceptions
{
    public class AccountNotFound : ExceptionToResponseProxy
    {
        public AccountNotFound() : base("Искомый счет не найден", false)
        {
            this.StatusCode = System.Net.HttpStatusCode.NotFound;
        }
    }
}
