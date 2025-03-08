using AutoMapper;
using CoreService.Helpers;
using CoreService.Helpers.Cache;
using CoreService.Integrations.Http.UserService;
using CoreService.Models.Cache;
using CoreService.Models.Database.Entity;
using CoreService.Models.DTO;
using CoreService.Models.Exceptions;
using CoreService.Models.Request.Account;
using CoreService.Models.Request.User;
using CoreService.Models.Response.User;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Caching.Memory;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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
        private string ExtractUserPhone(string token)
        {
            var decipheredToken = new JwtSecurityToken(token);
            var phone = decipheredToken.Claims.Where(x => x.Type == ClaimTypes.MobilePhone).FirstOrDefault();
            if (phone == null)
            {
                throw new TokenClaimsUnprocessable();
            }
            Console.WriteLine(phone.ToString());
            return phone.ToString();
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (httpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                string token = authHeader.ToString().Replace("Bearer ", "");
                var phone = this.ExtractUserPhone(token);
                var userParameters = _cache.GetUserParametersFromCache(phone);
                if (userParameters == null)
                {
                    var getCurrentClientResponse = await _userService.GetCurrentUser(httpContext, new GetCurrentUserRequest
                    {
                        BearerToken = token
                    });
                    userParameters = _mapper.Map<UserParametersCacheEntry>(getCurrentClientResponse);
                    _cache.InsertUserParametersIntoCache(phone, userParameters);
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
