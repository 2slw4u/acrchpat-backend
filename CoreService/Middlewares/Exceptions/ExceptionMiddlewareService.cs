using CoreService.Helpers;
using CoreService.Models.Exceptions;

namespace CoreService.Middlewares.Exceptions
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
                await ExceptionHandler.HandleHttpException(context, ex);
            }
            catch (Exception ex)
            {
                await ExceptionHandler.HandleSystemException(context, ex);                
            }
        }
    }
}
