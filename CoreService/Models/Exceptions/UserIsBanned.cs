namespace CoreService.Models.Exceptions
{
    public class UserIsBanned : ExceptionToResponseProxy
    {
        public UserIsBanned() : base("Пользователь забанен", false)
        {
            this.StatusCode = System.Net.HttpStatusCode.Forbidden;
        }
    }
}
