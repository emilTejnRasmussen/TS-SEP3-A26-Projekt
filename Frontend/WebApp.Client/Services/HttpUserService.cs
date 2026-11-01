using System.Net.Http.Json;
using WebApp.Client.DTO.User;

namespace WebApp.Client.Services;

public class HttpUserService(HttpClient client) : IUserService
{
    // GET /api/v1/Users/{id}
    public async Task<UserResponse> GetByIdAsync(long id)
    {
        var response = await client.GetAsync($"{ApiEndpoints.Users}/{id}");
        await ApiException.ThrowIfFailedAsync(response);
        return (await response.Content.ReadFromJsonAsync<UserResponse>())!;
    }

    // GET /api/v1/Users?nameContains=...
    public async Task<List<UserResponse>> GetAllAsync(string? nameContains = null)
    {
        var url = string.IsNullOrWhiteSpace(nameContains)
            ? ApiEndpoints.Users
            : $"{ApiEndpoints.Users}?nameContains={Uri.EscapeDataString(nameContains)}";

        var response = await client.GetAsync(url);
        await ApiException.ThrowIfFailedAsync(response);
        return await response.Content.ReadFromJsonAsync<List<UserResponse>>() ?? [];
    }
}
