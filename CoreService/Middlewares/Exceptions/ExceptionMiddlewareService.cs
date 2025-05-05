using CoreService.Helpers;
using CoreService.Models.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;
using CoreService.Helpers;

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
                if (!context.Response.HasStarted) await ExceptionHandler.HandleHttpException(context, ex);
                throw ex;
            }
            catch (Exception ex)
            {
                if (!context.Response.HasStarted) await ExceptionHandler.HandleSystemException(context, ex);
                throw ex;
            }
        }
    }
}
