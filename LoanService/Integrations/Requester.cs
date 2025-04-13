using System.Net.Http.Headers;
using System.Text;
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
        return url;
    }

    private void AddAuthorizationHeader(string? token = null)
    {
        token ??= GetAccessTokenFromContext();

        HttpClient.DefaultRequestHeaders.Authorization = token != null
            ? new AuthenticationHeaderValue("Bearer", token)
            : null;
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

    public async Task<T> GetAsync<T>(string endpoint)
    {
        AddAuthorizationHeader();
        var url = $"{GetServiceBaseUrl()}/{endpoint}";
        var response = await HttpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestFailedException(
                response.StatusCode,
                $"GET {url} failed with status code {response.StatusCode}: {await response.Content.ReadAsStringAsync()}"
                );
        }

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
               ?? throw new JsonException("Failed to deserialize response");
    }

    // public async Task<T> PostAsync<T>(string serviceName, string endpoint, object body, string? token = null)
    // {
    //     AddAuthorizationHeader(token);
    //     var url = $"{GetServiceBaseUrl(serviceName)}/{endpoint.TrimStart('/')}";
    //     var json = JsonSerializer.Serialize(body);
    //     var content = new StringContent(json, Encoding.UTF8, "application/json");
    //
    //     var response = await _httpClient.PostAsync(url, content);
    //
    //     if (!response.IsSuccessStatusCode)
    //     {
    //         throw new HttpRequestException($"POST {url} failed: {await response.Content.ReadAsStringAsync()}");
    //     }
    //
    //     var responseBody = await response.Content.ReadAsStringAsync();
    //     return JsonSerializer.Deserialize<T>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
    //            ?? throw new JsonException("Failed to deserialize response");
    // }
}