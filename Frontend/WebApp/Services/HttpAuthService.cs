using System.Net.Http.Json;
using DTO.Auth;

namespace WebApp.Services;

public class HttpAuthService(HttpClient client, TokenStore tokens, JwtAuthStateProvider authState) : IAuthService
{
    // POST /api/v1/Auth/register - opretter brugeren og logger ind med det samme.
    public async Task RegisterAsync(RegisterRequest request)
    {
        var response = await client.PostAsJsonAsync(ApiEndpoints.Register, request);
        await ApiException.ThrowIfFailedAsync(response);
        await SaveTokenAsync(response);
    }

    // POST /api/v1/Auth/login
    public async Task LoginAsync(LoginRequest request)
    {
        var response = await client.PostAsJsonAsync(ApiEndpoints.Login, request);
        await ApiException.ThrowIfFailedAsync(response);
        await SaveTokenAsync(response);
    }

    public async Task LogoutAsync()
    {
        await tokens.RemoveAsync();
        authState.NotifyChanged();
    }

    private async Task SaveTokenAsync(HttpResponseMessage response)
    {
        var auth = (await response.Content.ReadFromJsonAsync<AuthResponse>())!;
        await tokens.SetAsync(auth.Token);
        authState.NotifyChanged();
    }
}
