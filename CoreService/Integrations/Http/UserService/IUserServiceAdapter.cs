using CoreService.Models.Request.User;
using CoreService.Models.Response.User;

namespace CoreService.Integrations.Http.UserService
{
    public interface IUserServiceAdapter
    {
        Task<GetCurrentUserResponse> GetCurrentUser(HttpContext httpContext, GetCurrentUserRequest request);
    }
}
