using System.Security.Cryptography;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;

namespace WebApp.Services;

// Gemmer JWT'en i browserens localStorage, så man forbliver logget ind efter genindlæsning.
// ProtectedLocalStorage krypterer værdien med ASP.NET Core Data Protection på serveren,
// så JavaScript i browseren ikke kan læse en brugbar JWT.
// Scoped = én instans pr. kredsløb (bruger/fane). Tokenet caches i hukommelsen, så
// AuthHeaderHandler ikke skal spørge browseren ved hvert kald.
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
            // Gammel ukrypteret værdi eller skiftede nøgler - smid den væk og log ud.
            await RemoveAsync();
        }
        catch (Exception e) when (e is InvalidOperationException or JSDisconnectedException)
        {
            // Ingen forbindelse til browseren (endnu). Opfør os, som om man ikke er logget ind.
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
