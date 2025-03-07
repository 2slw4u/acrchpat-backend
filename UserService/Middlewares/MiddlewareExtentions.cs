namespace UserService.Middlewares
{
	public static class MiddlewareExtensions
	{
		public static void UseExceptionMiddleware(this IApplicationBuilder app)
		{
			app.UseMiddleware<ExceptionMiddlewareService>();
			//app.UseMiddleware<AuthorizationMiddlewareService>();
		}
	}
}
