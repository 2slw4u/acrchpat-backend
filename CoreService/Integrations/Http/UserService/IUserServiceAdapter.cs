using CoreService.Models.Http.Request.User;
using CoreService.Models.Http.Response.User;

namespace CoreService.Integrations.Http.UserService
{
    public interface IUserServiceAdapter
    {
        Task<GetCurrentUserResponse> GetCurrentUser(HttpContext httpContext, GetCurrentUserRequest request);
    }
}
