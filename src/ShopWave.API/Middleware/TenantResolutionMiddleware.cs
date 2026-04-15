using System.IdentityModel.Tokens.Jwt;
using ShopWave.Infrastructure.Services;

namespace ShopWave.API.Middleware;

public class TenantResolutionMiddleware(RequestDelegate next)
{
    private static readonly HashSet<string> _publicPaths =
    [
        "/api/auth/register",
        "/api/auth/login",
        "/api/auth/refresh"
    ];

    public async Task InvokeAsync(HttpContext context, TenantContext tenantContext)
    {
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? string.Empty;

        if (_publicPaths.Contains(path) || path.StartsWith("/swagger"))
        {
            await next(context);
            return;
        }

        var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
        if (authHeader?.StartsWith("Bearer ") == true)
        {
            var token = authHeader["Bearer ".Length..].Trim();
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);
                var tenantIdStr = jwt.Claims.FirstOrDefault(c => c.Type == "tenantId")?.Value;
                if (Guid.TryParse(tenantIdStr, out var tenantId))
                    tenantContext.SetTenantId(tenantId);
            }
            catch
            {
                // Let the auth middleware handle invalid tokens
            }
        }

        await next(context);
    }
}
