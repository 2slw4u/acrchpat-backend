using UserService.Middlewares.Authorization;
using UserService.Middlewares.Exception;

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
	}
}
