using LoanService.Models.General;

namespace LoanService.Middleware;

public class FailureImitatorMiddleware(RequestDelegate next)
{
    private readonly Random _random = new();
    public async Task InvokeAsync(HttpContext context)
    {
        int probability = DateTime.Now.Minute % 2 == 0 ? 10 : 50;
        if (_random.Next(101) > probability)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new ResponseModel
            {
                Status = "Unknown Server Error",
                Message = "You are unlucky"
            });
            return;
        }
        
        await next(context);
    }
}