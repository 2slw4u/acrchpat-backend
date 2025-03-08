using System.Net;

namespace CoreService.Models.Exceptions
{
    public class TokenClaimsUnprocessable : ExceptionToResponseProxy
    {
        public TokenClaimsUnprocessable() : base("Не смогли получить данные из токена", HttpStatusCode.Forbidden) { }
        public TokenClaimsUnprocessable(string message, HttpStatusCode statusCode = HttpStatusCode.Forbidden) : base(message) { }
    }
}
