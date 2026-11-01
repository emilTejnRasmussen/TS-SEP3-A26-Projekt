using Microsoft.JSInterop;

namespace WebApp.Client.Services;

// Gemmer JWT'en i browserens localStorage, så man forbliver logget ind,
// også når InteractiveAuto skifter fra server til WebAssembly.
public class TokenStore(IJSRuntime js)
{
    private const string Key = "authToken";

    // Under server-prerendering findes der ingen browser, så JS-kald fejler.
    // Så opfører vi os bare, som om man ikke er logget ind.
    public async Task<string?> GetAsync()
    {
        try
        {
            return await js.InvokeAsync<string?>("localStorage.getItem", Key);
        }
        catch (Exception e) when (e is InvalidOperationException or JSDisconnectedException)
        {
            return null;
        }
    }

    public async Task SetAsync(string token) =>
        await js.InvokeVoidAsync("localStorage.setItem", Key, token);

    public async Task RemoveAsync() =>
        await js.InvokeVoidAsync("localStorage.removeItem", Key);
}
