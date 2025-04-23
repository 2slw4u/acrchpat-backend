using LoanService.Exceptions;
using LoanService.Integrations;
using LoanService.Models.General;

namespace LoanService.Middleware;

public class AuthorizationMiddleware(RequestDelegate next, UserRequester userRequester)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value;
        
        if (path != null && (path.StartsWith("/swagger") || path.StartsWith("/favicon.ico")))
        {
            await next(context);
            return;
        }
        
        try
        {
            var userData = await userRequester.GetCurrentUserAsync();
            
            context.Items["UserId"] = userData.Id;
            context.Items["Roles"] = userData.Roles;
        }
        catch (HttpRequestFailedException ex)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new ResponseModel
            {
                Status = "Authorization error",
                Message = ex.Message
            });
            return;
        }
        catch (Exception e)
        {
            context.Response.StatusCode = StatusCodes.Status504GatewayTimeout;
            await context.Response.WriteAsJsonAsync(new ResponseModel
            {
                Status = "Authorization service connection error",
                Message = e.Message
            });
            return;
        }

        await next(context);
    }
}