using System.Security.Cryptography;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;

namespace WebApp.Services;

// ProtectedLocalStorage krypterer JWT'en, så JavaScript i browseren ikke kan læse den.
public class TokenStore(ProtectedLocalStorage storage)
{
    private const string Key = "authToken";

    private string? token;
    private bool loaded;

    public async Task<string?> GetAsync()
    {
        if (loaded) return token;

        try
        {
            var result = await storage.GetAsync<string>(Key);
            token = result.Success ? result.Value : null;
            loaded = true;
        }
        catch (CryptographicException)
        {
            // Gammel ukrypteret værdi eller skiftede nøgler.
            await RemoveAsync();
        }
        catch (Exception e) when (e is InvalidOperationException or JSDisconnectedException)
        {
            return null;
        }

        return token;
    }

    public async Task SetAsync(string value)
    {
        token = value;
        loaded = true;
        await storage.SetAsync(Key, value);
    }

    public async Task RemoveAsync()
    {
        token = null;
        loaded = true;
        await storage.DeleteAsync(Key);
    }
}
