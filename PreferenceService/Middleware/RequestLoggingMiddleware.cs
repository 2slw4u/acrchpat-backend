using System.Text;
using System.Text.Json;

namespace PreferenceService.Middleware;

public class RequestLoggingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value;
        
        if (path == "" || path != null && (path.StartsWith("/swagger") || path.StartsWith("/favicon.ico")))
        {
            await next(context);
            return;
        }
        
        Console.WriteLine($"{context.Request.Protocol} {context.Request.Method}" +
                          $" request to {context.Request.Path} from {context.Request.Host}" +
                          $" with body:\n{await BodyToStringAsync(context)}");
        
        await next(context);
    }

    private async Task<string> BodyToStringAsync(HttpContext context)
    {
        context.Request.EnableBuffering();

        context.Request.Body.Position = 0;

        using var reader = new StreamReader(
            context.Request.Body,
            encoding: Encoding.UTF8,
            detectEncodingFromByteOrderMarks: false,
            bufferSize: 1024,
            leaveOpen: true);

        var body = await reader.ReadToEndAsync();

        context.Request.Body.Position = 0;

        return body;
    }
    
    private string QueryToJson(HttpContext context)
    {
        var query = context.Request.Query
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.ToString()
            );

        var json = JsonSerializer.Serialize(query);
        return json;
    }
}