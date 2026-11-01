using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Entities;
using Microsoft.IdentityModel.Tokens;
using Service.Interface;

namespace WebApi.Authentication;

public class JwtTokenService(IConfiguration configuration) : ITokenService
{
    public string GenerateToken(User user)
    {
        var issuer = GetRequiredSetting("Jwt:Issuer");
        var audience = GetRequiredSetting("Jwt:Audience");
        var expirationMinutes = GetExpirationMinutes();

        var credentials = CreateSigningCredentials();
        var claims = CreateClaims(user);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GetRequiredSetting(string key)
    {
        var value = configuration[key];

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Missing JWT configuration: {key}");
        }

        return value;
    }

    private int GetExpirationMinutes()
    {
        var value = GetRequiredSetting("Jwt:ExpirationMinutes");

        if (!int.TryParse(value, out var minutes) || minutes <= 0)
        {
            throw new InvalidOperationException("JWT expiration must be a positive integer.");
        }

        return minutes;
    }

    private SigningCredentials CreateSigningCredentials()
    {
        var secret = GetRequiredSetting("Jwt:Key");
        var keyBytes = Convert.FromBase64String(secret);

        if (keyBytes.Length < 32)
        {
            throw new InvalidOperationException("JWT signing key must be at least 32 bytes.");
        }

        var key = new SymmetricSecurityKey(keyBytes);

        return new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256
        );
    }

    private static IEnumerable<Claim> CreateClaims(User user)
    {
        return
        [
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name, user.Name)
        ];
    }
}