using LoanService.Exceptions;
using LoanService.Integrations;
using LoanService.Models.General;

namespace LoanService.Middleware;

public class AuthorizationMiddleware(RequestDelegate next, UserRequester userRequester)
{
    private Random _random = new ();
    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value;
        
        if (path != null && (path.StartsWith("/swagger") || path.StartsWith("/favicon.ico")))
        {
            await next(context);
            return;
        }

        // int probability = DateTime.Now.Minute % 2 == 0 ? 10 : 50;
        // if (_random.Next(101) > probability)
        // {
        //     context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        //     await context.Response.WriteAsJsonAsync(new ResponseModel
        //     {
        //         Status = "Unknown Server Error",
        //         Message = "You are unlucky"
        //     });
        //     return;
        // }

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