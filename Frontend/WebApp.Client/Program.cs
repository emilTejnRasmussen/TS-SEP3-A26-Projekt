using WebApp.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Base-URL læses fra wwwroot/appsettings.json
builder.Services.AddBackendServices(builder.Configuration);

await builder.Build().RunAsync();
