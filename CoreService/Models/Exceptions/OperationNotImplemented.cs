namespace CoreService.Models.Exceptions
{
    public class OperationNotImplemented : ExceptionToResponseProxy
    {
        public OperationNotImplemented() : base("Данная операция ещё не внедрена", false)
        {
            this.StatusCode = System.Net.HttpStatusCode.NotImplemented;
        }
    }
}
