using UserService.Models.Exceptions;

namespace UserService.Middlewares.Exception
{
	public class ExceptionMiddlewareService
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<ExceptionMiddlewareService> _logger;
		public ExceptionMiddlewareService(RequestDelegate next, ILogger<ExceptionMiddlewareService> logger)
		{
			_next = next;
			_logger = logger;
		}
		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (NotFoundException ex)
			{
				context.Response.StatusCode = StatusCodes.Status404NotFound;
				await context.Response.WriteAsJsonAsync(new { message = "404 Not Found: " + ex.Message });
			}
			catch (ForbiddenException ex)
			{
				context.Response.StatusCode = StatusCodes.Status403Forbidden;
				await context.Response.WriteAsJsonAsync(new { message = "403 Forbidden: " + ex.Message });
			}
			catch (BadRequestException ex)
			{
				context.Response.StatusCode = StatusCodes.Status400BadRequest;
				await context.Response.WriteAsJsonAsync(new { message = "400 BadRequst: " + ex.Message });
			}
			catch (UnauthorizedException ex)
			{
				context.Response.StatusCode = StatusCodes.Status401Unauthorized;
				await context.Response.WriteAsJsonAsync(new { message = "401 Unauthorized: " + ex.Message });
			}
			//catch (Exception ex)
			//{
			//	context.Response.StatusCode = StatusCodes.Status500InternalServerError;
			//	_logger.LogError(ex.Message);
			//	await context.Response.WriteAsJsonAsync(new { message = "500 Internal Server Error:" + ex.Message });
			//}
		}
	}
}
