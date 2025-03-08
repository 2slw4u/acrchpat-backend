using LoanService.Exceptions;
using LoanService.Models.General;

namespace LoanService.Middleware;

public class ExceptionMiddlewareService(RequestDelegate next, ILogger<ExceptionMiddlewareService> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ResourceNotFoundException ex)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(new ResponseModel { Status = "Not Found Error", Message = ex.Message });
        }
        catch (AccessDeniedException ex)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new ResponseModel { Status = "Access Forbidden Error", Message = ex.Message });
        }
        catch (BadRequestException ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new ResponseModel { Status = "Bad Request Error", Message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new ResponseModel { Status = "Unauthorized Access Error", Message = ex.Message });
        }
        catch (InvalidCastException ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new ResponseModel { Status = "Guid Parsing Error", Message = ex.Message });
        }
        catch (CannotAcquireException ex)
        {
            context.Response.StatusCode = StatusCodes.Status504GatewayTimeout;
            await context.Response.WriteAsJsonAsync(new ResponseModel { Status = "Cannot Acquire Data Error", Message = ex.Message });
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            logger.LogError(ex.Message);
            await context.Response.WriteAsJsonAsync(new ResponseModel { Status = "Internal Server Error", Message = ex.Message });
        }
    }
}