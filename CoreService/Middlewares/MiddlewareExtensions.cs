using CoreService.Middlewares.Exceptions;
using CoreService.Middlewares.Authorization;
using CoreService.Middlewares.Monitoring;

namespace CoreService.Middlewares
{
    public static class MiddlewareExtensions
    {
        public static void UseExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddlewareService>();
        }

        public static void UseAuthorizationMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<AuthorizationMiddlewareService>();
        }

        public static void UseMonitoringMiddlewareService(this IApplicationBuilder app)
        {
            app.UseMiddleware<MonitoringMiddlewareService>();
        }
    }
}
