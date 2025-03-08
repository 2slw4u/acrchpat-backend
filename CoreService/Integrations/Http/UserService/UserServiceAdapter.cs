using CoreService.Models.Exceptions;
using CoreService.Models.Request.User;
using CoreService.Models.Response.User;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace CoreService.Integrations.Http.UserService
{
    public class UserServiceAdapter : IUserServiceAdapter
    {
        private readonly IConfiguration _configuration;
        private readonly string serviceUrl;
        public UserServiceAdapter(IConfiguration configuration)
        {
            _configuration = configuration;
            serviceUrl = _configuration["Integrations:Http:UserService:Api"];
        }
        public async Task<GetCurrentUserResponse> GetCurrentUser(HttpContext httpContext, GetCurrentUserRequest request)
        {
            try
            {
                HttpClient client = new();
                string url = $"{serviceUrl}{_configuration["Integrations:Http:UserService:GetCurrentUserOperationRoute"]}";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", request.BearerToken);
                HttpResponseMessage authResponse = await client.GetAsync(url);

                if (authResponse.StatusCode == HttpStatusCode.Forbidden)
                {
                    return new GetCurrentUserResponse { IsBanned = true };
                }
                if (!authResponse.IsSuccessStatusCode)
                {
                    throw new UserServiceError(await authResponse.Content.ReadAsStringAsync());
                }

                var responseBody = await authResponse.Content.ReadAsStringAsync();
                var userData = JsonSerializer.Deserialize<GetCurrentUserResponse>(responseBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return userData;
            }
            catch (Exception ex)
            {
                throw new UserServiceError(ex.Message);
            }
        }
    }
}
