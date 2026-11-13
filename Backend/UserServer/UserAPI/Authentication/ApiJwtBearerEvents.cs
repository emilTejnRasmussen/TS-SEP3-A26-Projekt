using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace WebApi.Authentication;

public class ApiJwtBearerEvents : JwtBearerEvents
{
    public override Task TokenValidated(TokenValidatedContext context)
    {
        if (context.Principal is null)
        {
            context.Fail("Invalid user identity.");
            return Task.CompletedTask;
        }

        try
        {
            context.Principal.GetUserId();
        }
        catch (UnauthorizedAccessException)
        {
            context.Fail("Invalid user identity.");
        }

        return Task.CompletedTask;
    }

    public override Task Challenge(JwtBearerChallengeContext context)
    {
        context.HandleResponse();
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.Headers.WWWAuthenticate = "Bearer";

        return context.Response.WriteAsJsonAsync(
            new { error = "Authentication is required. Please log in again." },
            context.HttpContext.RequestAborted);
    }

    public override Task Forbidden(ForbiddenContext context)
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;

        return context.Response.WriteAsJsonAsync(
            new { error = "You do not have permission to perform this operation." },
            context.HttpContext.RequestAborted);
    }
}
