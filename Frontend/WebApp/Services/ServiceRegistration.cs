using Microsoft.AspNetCore.Components.Authorization;

namespace WebApp.Services;

public static class ServiceRegistration
{
    // Kaldes fra Program.cs. Med Interactive Server svarer et DI-scope til ét kredsløb (circuit),
    // dvs. én bruger i én browserfane. Derfor er alt, der rører login-tilstanden, Scoped - aldrig Singleton,
    // ellers ville brugere dele hinandens JWT.
    public static IServiceCollection AddBackendServices(this IServiceCollection services, IConfiguration config)
    {
        var baseUrl = config["ApiBaseUrl"]
                      ?? throw new InvalidOperationException("ApiBaseUrl mangler i appsettings.json");

        // Login-tilstand (krypteret JWT i browserens localStorage) til AuthorizeView m.m.
        services.AddAuthorizationCore();
        services.AddCascadingAuthenticationState();
        services.AddScoped<TokenStore>();
        services.AddScoped<JwtAuthStateProvider>();
        services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<JwtAuthStateProvider>());

        // Én fælles forbindelses-pulje til backenden for alle brugere (undgår at løbe tør for sockets).
        services.AddSingleton(new SocketsHttpHandler { PooledConnectionLifetime = TimeSpan.FromMinutes(2) });

        // HttpClient pr. kredsløb, så AuthHeaderHandler bruger netop denne brugers JWT.
        // disposeHandler: false, så den fælles SocketsHttpHandler ikke lukkes, når kredsløbet slutter.
        // JWT'en sendes kun til ApiBaseUrl, fordi alle services bruger relative ruter fra ApiEndpoints.
        services.AddScoped(sp => new HttpClient(
            new AuthHeaderHandler(sp.GetRequiredService<TokenStore>())
            {
                InnerHandler = sp.GetRequiredService<SocketsHttpHandler>()
            },
            disposeHandler: false)
        {
            BaseAddress = new Uri(baseUrl)
        });

        services.AddScoped<IAuthService, HttpAuthService>();
        services.AddScoped<IUserService, HttpUserService>();
        services.AddScoped<IEventService, HttpEventService>();
        return services;
    }
}
