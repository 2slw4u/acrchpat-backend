

using NotificationService.Models;

namespace NotificationService.Integrations;

public class UserRequester : Requester
{
    protected override string ServiceName => "UserService";
    private const string UserControllerName = "user";

    public UserRequester(IConfiguration configuration, IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor)
        : base(configuration, httpClientFactory, httpContextAccessor)
    {
    }

    public async Task<UserResponse> GetUserAsync(Guid userId)
    {
        return await GetAsync<UserResponse>($"{UserControllerName}/user/{userId}");
    }
    
    public async Task<UserResponse> GetCurrentUserAsync()
    {
        return await GetAsync<UserResponse>($"{UserControllerName}/currentUser");
    }
}