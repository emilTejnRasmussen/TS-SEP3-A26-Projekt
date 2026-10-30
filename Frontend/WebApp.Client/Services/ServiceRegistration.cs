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

        services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(baseUrl) });
        services.AddScoped<IUserService, HttpUserService>();
        services.AddScoped<IEventService, HttpEventService>();
        return services;
    }
}
