using CoreService.Models.DTO;
using CoreService.Models.Exceptions;

namespace CoreService.Helpers
{
    public static class ContextDataHelper
    {
        public static Guid GetUserId(HttpContext httpContext)
        {
            var userId = httpContext.Items["UserId"] == null ? Guid.Empty : (Guid)httpContext.Items["UserId"];
            if (userId == Guid.Empty)
            {
                throw new DataInContextNotFound();
            }
            return userId;
        }

        public static void SetUserId(HttpContext httpContext, Guid? userId)
        {
            if (userId != null)
            {
                httpContext.Items["UserId"] = userId;
            }
        }

        public static List<UserRoleDTO> GetUserRoles(HttpContext httpContext)
        {
            var userId = httpContext.Items["UserRoles"] == null ? null : (List<UserRoleDTO>)httpContext.Items["UserRoles"];
            if (userId == null)
            {
                throw new DataInContextNotFound();
            }
            return userId;
        }

        public static void SetUserRoles(HttpContext httpContext,  List<UserRoleDTO>? userRoles)
        {
            if (userRoles != null)
            {
                httpContext.Items["UserRoles"] = userRoles;
            }
        }
    }
}
