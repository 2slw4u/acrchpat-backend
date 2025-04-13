using UserService.Models.Exceptions;
using UserService.Services.Interfaces;

namespace UserService.Middlewares.Authorization
{
	public class AuthorizationMiddlewareService
	{
		private readonly RequestDelegate _next;

		public AuthorizationMiddlewareService(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			if (context.User?.Identity?.IsAuthenticated == true)
			{
				using (var scope = context.RequestServices.CreateScope())
				{
					var authService = scope.ServiceProvider.GetRequiredService<IAuthenticationService>();

					var user = await  authService.GetCurrentUser();
					if (user.Bans.Any(b => b.BanEnd == null))
					{
						throw new ForbiddenException("User is banned");
					}
					if (user.Roles.Count == 0)
					{
						throw new ForbiddenException("Invalid role configuration");
					}
					
				}
			}

			await _next(context);
		}
	}
}
