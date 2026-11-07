using Microsoft.AspNetCore.DataProtection;
using WebApp.Components;
using WebApp.Services;

var builder = WebApplication.CreateBuilder(args);

/* Nøglerne, der krypterer JWT'en i browserens localStorage (ProtectedLocalStorage),
gemmes på disk, så brugerne ikke bliver logget ud,
når serveren genstartes. Stien kan ændres med "DataProtection:KeysPath".
 */
var keysPath = builder.Configuration["DataProtection:KeysPath"]
               ?? Path.Combine(builder.Environment.ContentRootPath, "dp-keys");
builder.Services.AddDataProtection()
    .SetApplicationName("KIN.WebApp")
    .PersistKeysToFileSystem(new DirectoryInfo(keysPath));

// Interactive Server: komponenterne kører på serveren, og browseren får UI-opdateringer over en SignalR-forbindelse.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// HttpClient, login-tilstand og backend-services. Base-URL læses fra appsettings.json.
builder.Services.AddBackendServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
