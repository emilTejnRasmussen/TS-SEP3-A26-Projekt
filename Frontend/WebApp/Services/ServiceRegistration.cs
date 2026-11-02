using Microsoft.AspNetCore.Components.Authorization;

namespace WebApp.Services;

public static class ServiceRegistration
{
    // Ét scope = ét kredsløb (én bruger i én fane). Login-tilstand skal være Scoped, aldrig Singleton,
    // ellers deler brugerne hinandens JWT.
    public static IServiceCollection AddBackendServices(this IServiceCollection services, IConfiguration config)
    {
        var baseUrl = config["ApiBaseUrl"]
                      ?? throw new InvalidOperationException("ApiBaseUrl mangler i appsettings.json");

        services.AddAuthorizationCore();
        services.AddCascadingAuthenticationState();
        services.AddScoped<TokenStore>();
        services.AddScoped<JwtAuthStateProvider>();
        services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<JwtAuthStateProvider>());

        // Én fælles forbindelses-pulje for alle brugere, så serveren ikke løber tør for sockets.
        services.AddSingleton(new SocketsHttpHandler { PooledConnectionLifetime = TimeSpan.FromMinutes(2) });

        // disposeHandler: false, så den fælles SocketsHttpHandler ikke lukkes, når kredsløbet slutter.
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
