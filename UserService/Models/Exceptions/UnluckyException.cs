namespace UserService.Models.Exceptions
{
    public class UnluckyException : Exception
    {
        public UnluckyException(string message) : base(message) { }
    }
}
