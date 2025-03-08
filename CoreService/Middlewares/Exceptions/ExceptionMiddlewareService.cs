using CoreService.Helpers;
using CoreService.Models.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;

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
                ExceptionHandler.HandleHttpException(context, ex);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleSystemException(context, ex);                
            }
        }
    }
}
