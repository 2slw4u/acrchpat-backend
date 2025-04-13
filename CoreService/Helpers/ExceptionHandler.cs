using CoreService.Models.Exceptions;

namespace CoreService.Helpers
{
    public static class ExceptionHandler
    {
        public static async Task HandleSystemException(HttpContext context, Exception ex)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new { message = $"500Internal Server Error. Someone somewhere did an oopsie: {ex.Message}" });
            return;
        }
        public static async Task HandleHttpException(HttpContext context, ExceptionToResponseProxy ex)
        {
            context.Response.StatusCode = (int)ex.StatusCode;
            await context.Response.WriteAsJsonAsync(new { message = $"{(int)ex.StatusCode} {ex.StatusCode}: {ex.Message}" });
            return;
        }
    }
}
