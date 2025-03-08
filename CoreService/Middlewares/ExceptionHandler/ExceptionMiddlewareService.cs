using CoreService.Models.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CoreService.Middlewares.ExceptionHandler
{
    public class ExceptionMiddlewareService
    {
        private readonly RequestDelegate _next;
        public ExceptionMiddlewareService(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ExceptionToResponseProxy ex)
            {
                context.Response.StatusCode = (int)ex.StatusCode;
                await context.Response.WriteAsJsonAsync(new { message = $"{(int)ex.StatusCode} {ex.StatusCode}: {ex.Message}" });
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new { message = $"500Internal Server Error. Someone somewhere did an oopsie: {ex.Message}" });
            }
        }
    }
}
