using System.Net.Http.Headers;
using System.Text.Json;
using LoanService.Models.General;

namespace LoanService.Middleware;

public class AuthorizationMiddleware(RequestDelegate next, IConfiguration configuration)
{
    private readonly string? _backendIp = configuration["Backend:VpaIp"];
    
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            string token = authHeader.ToString().Replace("Bearer ", "");

            try
            {
                HttpClient client = new();
                string url = $"http://{_backendIp}:5003/api/user/currentUser";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage authResponse = await client.GetAsync(url);
                
                if (!authResponse.IsSuccessStatusCode)
                {
                    context.Response.StatusCode = (int)authResponse.StatusCode;
                    foreach (var header in authResponse.Headers)
                    {
                        context.Response.Headers[header.Key] = string.Join(", ", header.Value);
                    }
                    var unauthorizedResponseBody = await authResponse.Content.ReadAsStringAsync();
                    await context.Response.WriteAsync(unauthorizedResponseBody);
                    return;
                }
                
                var responseBody = await authResponse.Content.ReadAsStringAsync();
                var userData = JsonSerializer.Deserialize<UserResponse>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (userData?.Id != null)
                {
                    context.Items["UserId"] = userData.Id;
                }
                if (userData?.Roles != null)
                {
                    context.Items["Roles"] = userData.Roles;
                }
            }
            catch (Exception e)
            {
                context.Response.StatusCode = StatusCodes.Status504GatewayTimeout;
                await context.Response.WriteAsJsonAsync(new ResponseModel
                {
                    Status = "Authorization service connection error",
                    Message = e.Message
                });
                return;
            }
        }

        await next(context);
    }
}