using System.Net.Http.Headers;

namespace WebApp.Client.Services;

// Sætter "Authorization: Bearer <jwt>" på alle kald til backenden, når man er logget ind.
public class AuthHeaderHandler(TokenStore tokens) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await tokens.GetAsync();
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
