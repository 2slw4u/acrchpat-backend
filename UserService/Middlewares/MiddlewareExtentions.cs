using UserService.Middlewares.Authorization;
using UserService.Middlewares.Exceptions;
using UserService.Middlewares.Monitoring;

namespace UserService.Middlewares
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
