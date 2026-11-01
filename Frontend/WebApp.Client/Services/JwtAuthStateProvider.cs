using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;

namespace WebApp.Client.Services;

// Fortæller Blazor (AuthorizeView, [CascadingParameter] Task<AuthenticationState>), hvem der er logget ind.
// Læser claims direkte fra JWT'en: "sub" (bruger-id), "email", "name" og "exp".
// Signaturen tjekkes IKKE her - det gør backenden. Frontenden bruger det kun til visning.
public class JwtAuthStateProvider(TokenStore tokens) : AuthenticationStateProvider
{
    private static readonly AuthenticationState Anonymous = new(new ClaimsPrincipal(new ClaimsIdentity()));

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await tokens.GetAsync();
        if (string.IsNullOrEmpty(token)) return Anonymous;

        var claims = ParseClaims(token);
        if (claims is null)
        {
            // Udløbet eller ugyldigt token - smid det væk.
            await tokens.RemoveAsync();
            return Anonymous;
        }

        var identity = new ClaimsIdentity(claims, authenticationType: "jwt", nameType: "name", roleType: "role");
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    // Kaldes efter login/logout, så menuen m.m. opdateres med det samme.
    public void NotifyChanged() => NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

    private static List<Claim>? ParseClaims(string jwt)
    {
        var parts = jwt.Split('.');
        if (parts.Length != 3) return null;

        try
        {
            var json = Encoding.UTF8.GetString(Base64UrlDecode(parts[1]));
            using var doc = JsonDocument.Parse(json);

            var claims = new List<Claim>();
            foreach (var property in doc.RootElement.EnumerateObject())
            {
                if (property.Value.ValueKind == JsonValueKind.Array)
                {
                    claims.AddRange(property.Value.EnumerateArray()
                        .Select(item => new Claim(property.Name, item.ToString())));
                }
                else
                {
                    claims.Add(new Claim(property.Name, property.Value.ToString()));
                }
            }

            var exp = claims.Find(c => c.Type == "exp");
            if (exp is not null && long.TryParse(exp.Value, out var seconds) &&
                DateTimeOffset.FromUnixTimeSeconds(seconds) <= DateTimeOffset.UtcNow)
            {
                return null;
            }

            return claims;
        }
        catch (Exception e) when (e is FormatException or JsonException)
        {
            return null;
        }
    }

    private static byte[] Base64UrlDecode(string input)
    {
        var base64 = input.Replace('-', '+').Replace('_', '/');
        base64 = base64.PadRight(base64.Length + (4 - base64.Length % 4) % 4, '=');
        return Convert.FromBase64String(base64);
    }
}
