namespace CoreService.Middlewares.ExceptionHandler
{
    public static class MiddlewareExtensions
    {
        public static void UseExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddlewareService>();
        }
    }
}
