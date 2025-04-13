using CoreService.Helpers;
using CoreService.Models.Enum;
using CoreService.Models.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CoreService.Attributes
{
    public class RoleAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly UserRole _requiredRole;
        public RoleAuthorizeAttribute(UserRole requiredRole) 
        {
            _requiredRole = requiredRole;
        }
        public async void OnAuthorization(AuthorizationFilterContext context)
        {
            var roles = ContextDataHelper.GetUserRoles(context.HttpContext);
            if (!roles.Any(x => x.Name.Equals(_requiredRole.ToString(), StringComparison.OrdinalIgnoreCase)))
            {
                await ExceptionHandler.HandleHttpException(context.HttpContext, new UserDoesntOwnRequiredRole());
            }
            return;
        }
    }
}
