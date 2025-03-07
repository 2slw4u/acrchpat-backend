using LoanService.Middleware;
using LoanService.Models.General;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LoanService.Attributes;

public class RoleAuthorizeAttribute(string requiredRole) : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var roles = context.HttpContext.Items["Roles"] as List<RoleDto>;

        if (roles == null || !roles.Any(r => r.Name.Equals(requiredRole, StringComparison.OrdinalIgnoreCase)))
        {
            context.Result = new ObjectResult(new ResponseModel
            {
                Status = "Access Denied Error",
                Message = $"You should be a {requiredRole}"
            })
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
        }
    }
}