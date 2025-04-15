using CoreService.Models.DTO;
using CoreService.Models.Exceptions;

namespace CoreService.Helpers
{
    public static class ContextDataHelper
    {
        private const string userIdPath = "UserId";
        private const string userRolesPath = "UserRoles";
        public static Guid GetUserId(HttpContext httpContext)
        {
            var userId = httpContext.Items[userIdPath] == null ? Guid.Empty : (Guid)httpContext.Items[userIdPath];
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
                httpContext.Items[userIdPath] = userId;
            }
        }

        public static List<UserRoleDTO> GetUserRoles(HttpContext httpContext)
        {
            var userRoles = httpContext.Items[userRolesPath] == null ? null : (List<UserRoleDTO>)httpContext.Items[userRolesPath];
            if (userRoles == null)
            {
                throw new DataInContextNotFound();
            }
            return userRoles;
        }

        public static void SetUserRoles(HttpContext httpContext,  List<UserRoleDTO>? userRoles)
        {
            if (userRoles != null)
            {
                httpContext.Items[userRolesPath] = userRoles;
            }
        }
    }
}
