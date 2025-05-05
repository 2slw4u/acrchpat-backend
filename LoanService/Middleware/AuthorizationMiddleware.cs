using LoanService.Exceptions;
using LoanService.Integrations;
using LoanService.Integrations.HttpRequesters;
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
            throw new HttpRequestFailedException(System.Net.HttpStatusCode.Unauthorized, "Authorization error");
            /*context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new ResponseModel
            {
                Status = "Authorization error",
                Message = ex.Message
            });
            return;*/
        }
        catch (Exception e)
        {
            throw new HttpRequestFailedException(System.Net.HttpStatusCode.GatewayTimeout, "Gateway timeout");
            /*context.Response.StatusCode = StatusCodes.Status504GatewayTimeout;
            await context.Response.WriteAsJsonAsync(new ResponseModel
            {
                Status = "Authorization service connection error",
                Message = e.Message
            });
            return;*/
        }

        await next(context);
    }
}