using System.Net.Http.Json;
using WebApp.Client.DTO.User;

namespace WebApp.Client.Services;

public class HttpUserService(HttpClient client) : IUserService
{
    // GET /api/v1/Users
    public async Task<List<UserResponse>> GetAllAsync()
    {
        var response = await client.GetAsync(ApiEndpoints.Users);
        await ApiException.ThrowIfFailedAsync(response);
        return await response.Content.ReadFromJsonAsync<List<UserResponse>>() ?? [];
    }
}
