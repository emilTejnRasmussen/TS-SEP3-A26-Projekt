using System.Net.Http.Json;
using DTO.User;

namespace WebApp.Services;

public class HttpUserService(HttpClient client) : IUserService
{
    public async Task<List<UserResponse>> GetAllAsync()
    {
        var response = await client.GetAsync(ApiEndpoints.Users);
        await ApiException.ThrowIfFailedAsync(response);
        return await response.Content.ReadFromJsonAsync<List<UserResponse>>() ?? [];
    }
}
