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
            await context.Response.WriteAsJsonAsync(new ResponseModel
                { Status = "Not Found Error", Message = ex.Message });
            throw ex;
        }
        catch (AccessDeniedException ex)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new ResponseModel
                { Status = "Access Forbidden Error", Message = ex.Message });
            throw ex;
        }
        catch (BadRequestException ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new ResponseModel
                { Status = "Bad Request Error", Message = ex.Message });
            throw ex;
        }
        catch (UnauthorizedAccessException ex)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new ResponseModel
                { Status = "Unauthorized Access Error", Message = ex.Message });
            throw ex;
        }
        catch (InvalidCastException ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new ResponseModel
                { Status = "Guid Parsing Error", Message = ex.Message });
            throw ex;
        }
        catch (CannotAcquireException ex)
        {
            context.Response.StatusCode = StatusCodes.Status504GatewayTimeout;
            await context.Response.WriteAsJsonAsync(new ResponseModel
                { Status = "Cannot Acquire Data Error", Message = ex.Message });
            throw ex;
        }
        catch (ConflictException ex)
        {
            context.Response.StatusCode = StatusCodes.Status409Conflict;
            await context.Response.WriteAsJsonAsync(new ResponseModel
                { Status = "Data Conflict Error", Message = ex.Message });
            throw ex;
        }
        catch (HttpRequestFailedException ex)
        {
            context.Response.StatusCode = (int)ex.StatusCode;
            await context.Response.WriteAsJsonAsync(new ResponseModel
                { Status = "Inner HTTP Request Failed", Message = ex.Message });
            throw ex;
        }
        catch (UnluckyException ex)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new ResponseModel
            { Status = "Internal Server Error", Message = ex.Message });
            throw ex;
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            logger.LogError(ex.Message);
            await context.Response.WriteAsJsonAsync(new ResponseModel { Status = "Internal Server Error", Message = ex.Message });
            throw ex;
        }
    }
}