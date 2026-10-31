using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace WebApi.Authentication;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal principal)
    {
        var value = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (!int.TryParse(value, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user identity.");
        }

        return userId;
    }
}