namespace LoanService.Exceptions
{
    public class UnluckyException : Exception
    {
        public UnluckyException()
        {
        }

        public UnluckyException(string message) : base(message)
        {
        }

        public UnluckyException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
