using NotificationService.Exceptions;
using NotificationService.Models;

namespace NotificationService.Middleware;

public class ExceptionMiddlewareService(RequestDelegate next, ILogger<ExceptionMiddlewareService> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (BadRequestException ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new ResponseModel
                { Status = "Bad Request Error", Message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new ResponseModel
                { Status = "Unauthorized Access Error", Message = ex.Message });
        }
        catch (HttpRequestFailedException ex)
        {
            context.Response.StatusCode = (int)ex.StatusCode;
            await context.Response.WriteAsJsonAsync(new ResponseModel
                { Status = "Inner HTTP Request Failed", Message = ex.Message });
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            logger.LogError(ex.Message);
            await context.Response.WriteAsJsonAsync(new ResponseModel { Status = "Internal Server Error", Message = ex.Message });
        }
    }
}