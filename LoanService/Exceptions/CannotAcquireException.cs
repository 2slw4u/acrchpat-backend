namespace LoanService.Exceptions;

public class CannotAcquireException : Exception
{
    public CannotAcquireException()
    {
    }

    public CannotAcquireException(string message) : base(message)
    {
    }

    public CannotAcquireException(string message, Exception innerException) : base(message, innerException)
    {
    }
}