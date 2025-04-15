using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using LoanService.Exceptions;

namespace LoanService.Integrations;

public class Requester
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly string? _backendIp;

    protected virtual string ServiceName => "Dummy";

    protected Requester(IConfiguration configuration, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _configuration = configuration;
        _httpClient = httpClientFactory.CreateClient();
        _backendIp = configuration["Backend:VpaIp"];
        _httpContextAccessor = httpContextAccessor;
    }

    private string GetServiceBaseUrl()
    {
        var port = _configuration[$"Services:{ServiceName}:Port"];
        var endpointPrefix = _configuration[$"Services:{ServiceName}:EndpointPrefix"];

        var url = $"http://{_backendIp}:{port}/{endpointPrefix}";
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new InvalidOperationException($"Base URL for service '{ServiceName}' is not configured.");
        }
        return url;
    }

    private void AddAuthorizationHeader(string? token = null)
    {
        token ??= GetAccessTokenFromContext();

        _httpClient.DefaultRequestHeaders.Authorization = token != null
            ? new AuthenticationHeaderValue("Bearer", token)
            : null;
    }
    
    private string? GetAccessTokenFromContext()
    {
        var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

        if (!string.IsNullOrWhiteSpace(authHeader) && authHeader.StartsWith("Bearer "))
        {
            return authHeader["Bearer ".Length..];
        }

        return null;
    }

    protected async Task<T> GetAsync<T>(string endpoint)
    {
        AddAuthorizationHeader();
        var url = $"{GetServiceBaseUrl()}/{endpoint}";
        var response = await _httpClient.GetAsync(url);
        var responseText = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestFailedException(
                response.StatusCode,
                $"GET {url} failed with status code {response.StatusCode}{(responseText.Length > 0 ? ": " : "")}{responseText}"
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