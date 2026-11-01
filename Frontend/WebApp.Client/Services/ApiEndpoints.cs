namespace WebApp.Client.Services;

// Alle backend-ruter samlet ét sted. Ret her, hvis backendens ruter ændres.
// Base-URL'en (fx http://localhost:5243/) står i "ApiBaseUrl" i appsettings.json.
// Backendens controllere arver fra ApiControllerBase: [Route("api/v1/[controller]")].
public static class ApiEndpoints
{
    private const string Prefix = "api/v1/";

    public const string Register = Prefix + "Auth/register";
    public const string Login = Prefix + "Auth/login";

    public const string Users = Prefix + "Users";

    // Event-kald går gennem User Server (som videresender til Event Server via gRPC).
    public const string Events = Prefix + "Events";
}
