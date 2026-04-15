using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ShopWave.Application.Common.Interfaces;
using ShopWave.Domain.Entities;

namespace ShopWave.Infrastructure.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly string _issuer;
    private readonly string _audience;
    private readonly SymmetricSecurityKey _signingKey;
    private readonly SigningCredentials _signingCredentials;
    private static readonly JwtSecurityTokenHandler TokenHandler = new();

    public int ExpiryMinutes { get; }
    public int RefreshTokenExpiryDays { get; }

    public JwtTokenService(IConfiguration configuration)
    {
        var secret = configuration["Jwt:Secret"]
            ?? throw new InvalidOperationException("Jwt:Secret is not configured.");

        _issuer = configuration["Jwt:Issuer"] ?? "ShopWave";
        _audience = configuration["Jwt:Audience"] ?? "ShopWave";
        ExpiryMinutes = int.TryParse(configuration["Jwt:ExpiryMinutes"], out var m) ? m : 60;
        RefreshTokenExpiryDays = int.TryParse(configuration["Jwt:RefreshTokenExpiryDays"], out var d) ? d : 30;

        _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        _signingCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
    }

    public string GenerateAccessToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("tenantId", user.TenantId.ToString()),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(ExpiryMinutes),
            signingCredentials: _signingCredentials);

        return TokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var bytes = new byte[64];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }

    public Guid? GetUserIdFromExpiredToken(string token)
    {
        var parameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = _signingKey,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false // intentionally allow expired tokens
        };

        try
        {
            var principal = TokenHandler.ValidateToken(token, parameters, out _);
            var userIdStr = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
            return Guid.TryParse(userIdStr, out var userId) ? userId : null;
        }
        catch (Exception ex) when (ex is SecurityTokenException or ArgumentException)
        {
            return null;
        }
    }
}
