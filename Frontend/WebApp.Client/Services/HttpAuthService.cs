using System.Net.Http.Json;
using Microsoft.JSInterop;
using WebApp.Client.DTO.Auth;

namespace WebApp.Client.Services;

// Gemmer JWT'en i browserens localStorage, så man forbliver logget ind,
// også når InteractiveAuto skifter fra server til WebAssembly.
// TODO: Tilføj AuthenticationStateProvider + Bearer-header, når der kommer endpoints med [Authorize].
public class HttpAuthService(HttpClient client, IJSRuntime js) : IAuthService
{
    private const string TokenKey = "authToken";

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

    public async Task LogoutAsync() =>
        await js.InvokeVoidAsync("localStorage.removeItem", TokenKey);

    public async Task<string?> GetTokenAsync() =>
        await js.InvokeAsync<string?>("localStorage.getItem", TokenKey);

    private async Task SaveTokenAsync(HttpResponseMessage response)
    {
        var auth = (await response.Content.ReadFromJsonAsync<AuthResponse>())!;
        await js.InvokeVoidAsync("localStorage.setItem", TokenKey, auth.Token);
    }
}
