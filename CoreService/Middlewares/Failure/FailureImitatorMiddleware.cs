using CoreService.Models.Exceptions;
using CoreService.Models.Http.Response.Error;

namespace CoreService.Middlewares.Failure;

public class FailureImitatorMiddleware(RequestDelegate next)
{
    private readonly Random _random = new();
    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value;
        if (path != null && (path.StartsWith("/swagger") || path.StartsWith("/favicon.ico")))
        {
            await next(context);
            return;
        }
        
        int probability = DateTime.Now.Minute % 2 == 0 ? 10 : 50;
        if (_random.Next(101) > probability)
        {
            /*context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new ResponseModel
            {
                Status = "Unknown Server Error",
                Message = "You are unlucky"
            });
            return;*/
            throw new Unlucky();
        }
        
        await next(context);
    }
}