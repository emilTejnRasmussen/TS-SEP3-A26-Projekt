namespace WebApp.Services;

public static class ApiEndpoints
{
    private const string Prefix = "api/v1/";

    public const string Register = Prefix + "Auth/register";
    public const string Login = Prefix + "Auth/login";

    public const string Users = Prefix + "Users";

    // Går gennem User Server, som videresender til Event Server via gRPC.
    public const string Events = Prefix + "Events";
}
