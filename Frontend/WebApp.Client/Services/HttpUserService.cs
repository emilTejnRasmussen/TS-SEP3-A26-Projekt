using System.Net.Http.Json;
using WebApp.Client.DTO.User;

namespace WebApp.Client.Services;

public class HttpUserService(HttpClient client) : IUserService
{
    // POST /Users
    public async Task<UserResponse> CreateAsync(CreateUserRequest request)
    {
        var response = await client.PostAsJsonAsync(ApiEndpoints.Users, request);
        await ApiException.ThrowIfFailedAsync(response);
        return (await response.Content.ReadFromJsonAsync<UserResponse>())!;
    }

    // GET /Users
    public async Task<List<UserResponse>> GetAllAsync()
    {
        var response = await client.GetAsync(ApiEndpoints.Users);
        await ApiException.ThrowIfFailedAsync(response);
        return await response.Content.ReadFromJsonAsync<List<UserResponse>>() ?? [];
    }
}
