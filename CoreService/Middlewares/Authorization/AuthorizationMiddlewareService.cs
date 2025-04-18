using AutoMapper;
using CoreService.Helpers;
using CoreService.Helpers.Cache;
using CoreService.Integrations.Http.UserService;
using CoreService.Models.Cache;
using CoreService.Models.Exceptions;
using CoreService.Models.Http.Request.User;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace CoreService.Middlewares.Authorization
{
    public class AuthorizationMiddlewareService
    {
        private readonly RequestDelegate _next;
        private readonly IUserParametersCache _cache;
        private readonly IUserServiceAdapter _userService;
        private readonly IMapper _mapper;
        public AuthorizationMiddlewareService(RequestDelegate next, 
            IUserParametersCache cache, 
            IUserServiceAdapter userService,
            IMapper mapper)
        {
            _next = next;
            _cache = cache;
            _userService = userService;
            _mapper = mapper;
        }
        // private string ExtractUserLogin(string token)
        // {
        //     var decipheredToken = new JwtSecurityToken(token);
        //     var login = decipheredToken.Claims.Where(x => x.Type == ClaimTypes.MobilePhone).FirstOrDefault().ToString();
        //     Regex regex = new Regex(@"(.*(mobilephone\:\s))");
        //     login = regex.Replace(login, "");
        //     Console.WriteLine($"phone in claims: {login}");
        //     if (login == null)
        //     {
        //         throw new TokenClaimsUnprocessable();
        //     }
        //     return login.ToString();
        // }
        
        private string ExtractUserLogin(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            if (!handler.CanReadToken(token))
            {
                throw new TokenClaimsUnprocessable();
            }

            var jwt = handler.ReadJwtToken(token);
            
            var loginClaim = jwt.Claims.FirstOrDefault(x => x.Type == ClaimTypes.MobilePhone);

            if (loginClaim == null || string.IsNullOrWhiteSpace(loginClaim.Value))
            {
                throw new TokenClaimsUnprocessable();
            }

            Console.WriteLine($"[Authorization] phone in claims: {loginClaim.Value}");

            return loginClaim.Value;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (httpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                string token = authHeader.ToString().Replace("Bearer ", "");
                var login = this.ExtractUserLogin(token);
                var userParameters = _cache.GetUserParametersFromCache(login);
                if (userParameters == null)
                {
                    var getCurrentClientResponse = await _userService.GetCurrentUser(httpContext, new GetCurrentUserRequest
                    {
                        BearerToken = token
                    });
                    userParameters = _mapper.Map<UserParametersCacheEntry>(getCurrentClientResponse);
                    _cache.InsertUserParametersIntoCache(login, userParameters);
                }
                if (userParameters.IsBanned)
                {
                    throw new UserIsBanned();
                }
                ContextDataHelper.SetUserRoles(httpContext, userParameters.Roles);
                ContextDataHelper.SetUserId(httpContext, userParameters.Id);
                Console.WriteLine($"Data from userService: {userParameters}" +
                    $"\nData saved to context: userId - {httpContext.Items["UserId"].ToString()}" +
                    $"\nAnd roles - {httpContext.Items["UserRoles"].ToString()}");
            }
            await _next(httpContext);
        }
    }
}
