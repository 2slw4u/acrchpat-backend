namespace LoanService.Middleware;

public class RequestLoggingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        Console.WriteLine(context.Request);
        
        await next(context);
    }
}