namespace LoanService.Exceptions
{
    public class RequestIsAlreadyProcessing : Exception
    {
        public RequestIsAlreadyProcessing()
        {
        }

        public RequestIsAlreadyProcessing(string message) : base(message)
        {
        }

        public RequestIsAlreadyProcessing(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
