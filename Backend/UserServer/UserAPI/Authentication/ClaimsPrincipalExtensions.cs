using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Globalization;

namespace WebApi.Authentication;

public static class ClaimsPrincipalExtensions
{
    public static long GetUserId(this ClaimsPrincipal principal)
    {
        var value = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (!long.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var userId)
            || userId <= 0)
        {
            throw new UnauthorizedAccessException("Invalid user identity.");
        }

        return userId;
    }
}
