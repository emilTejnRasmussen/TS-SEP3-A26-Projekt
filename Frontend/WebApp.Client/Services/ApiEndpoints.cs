namespace WebApp.Client.Services;

// Alle backend-ruter samlet ét sted. Ret her, hvis backendens ruter ændres.
// Base-URL'en (fx http://localhost:5243/) står i "ApiBaseUrl" i appsettings.json.
public static class ApiEndpoints
{
    // UsersController med [Route("[controller]")] -> /Users
    public const string Users = "Users";

    // Event-kald går gennem User Server (som videresender til Event Server via gRPC).
    // TODO: Ret, når backendens EventsController er lavet.
    public const string Events = "Events";
}
