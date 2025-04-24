using System.Net.Http.Headers;
using System.Text.Json;
using LoanService.Exceptions;

namespace LoanService.Integrations;

public class Requester
{
    protected readonly IConfiguration Configuration;
    protected readonly HttpClient HttpClient;
    protected readonly IHttpContextAccessor HttpContextAccessor;
    protected readonly string? BackendIp;

    protected virtual string ServiceName => "Dummy";

    public Requester(IConfiguration configuration, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        Configuration = configuration;
        HttpClient = httpClientFactory.CreateClient();
        BackendIp = configuration["Backend:VpaIp"];
        HttpContextAccessor = httpContextAccessor;
    }

    private string GetServiceBaseUrl()
    {
        var port = Configuration[$"Services:{ServiceName}:Port"];
        var endpointPrefix = Configuration[$"Services:{ServiceName}:EndpointPrefix"];

        var url = $"http://{BackendIp}:{port}/{endpointPrefix}";
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new InvalidOperationException($"Base URL for service '{ServiceName}' is not configured.");
        }
        return url.TrimEnd('/');
    }

    private string? GetAccessTokenFromContext()
    {
        var authHeader = HttpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

        if (!string.IsNullOrWhiteSpace(authHeader) && authHeader.StartsWith("Bearer "))
        {
            return authHeader["Bearer ".Length..];
        }

        return null;
    }

    protected async Task<T> GetAsync<T>(string endpoint)
    {
        var token = GetAccessTokenFromContext();
        var url = $"{GetServiceBaseUrl()}/{endpoint.TrimStart('/')}";

        using var request = new HttpRequestMessage(HttpMethod.Get, url);

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await HttpClient.SendAsync(request);
        var responseText = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"got data for {ServiceName}: {responseText}");

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestFailedException(
                response.StatusCode,
                $"GET {url} failed with status code {response.StatusCode}{(responseText.Length > 0 ? ": " : "")}{responseText}"
            );
        }

        return JsonSerializer.Deserialize<T>(responseText, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
               ?? throw new JsonException("Failed to deserialize response");
    }
}