using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebApp.Client.Services;

public static class ServiceRegistration
{
    // Kaldes fra BÅDE server- og client-projektets Program.cs, fordi InteractiveAuto
    // kører komponenterne først på serveren og derefter i browseren (WebAssembly).
    public static IServiceCollection AddBackendServices(this IServiceCollection services, IConfiguration config)
    {
        var baseUrl = config["ApiBaseUrl"]
                      ?? throw new InvalidOperationException("ApiBaseUrl mangler i appsettings.json");

        // Login-tilstand (JWT i localStorage) til AuthorizeView m.m.
        services.AddAuthorizationCore();
        services.AddCascadingAuthenticationState();
        services.AddScoped<TokenStore>();
        services.AddScoped<JwtAuthStateProvider>();
        services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<JwtAuthStateProvider>());

        // HttpClient sender JWT'en med på alle kald via AuthHeaderHandler.
        services.AddScoped(sp => new HttpClient(
            new AuthHeaderHandler(sp.GetRequiredService<TokenStore>()) { InnerHandler = new HttpClientHandler() })
        {
            BaseAddress = new Uri(baseUrl)
        });

        services.AddScoped<IAuthService, HttpAuthService>();
        services.AddScoped<IUserService, HttpUserService>();
        services.AddScoped<IEventService, HttpEventService>();
        return services;
    }
}
