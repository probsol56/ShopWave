using ShopWave.Infrastructure.Services;

namespace ShopWave.API.Middleware;

public class TenantResolutionMiddleware(RequestDelegate next, ILogger<TenantResolutionMiddleware> logger)
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

        // UseAuthentication() runs before this middleware and has already validated
        // the JWT signature, expiry, issuer, and audience. Read from the validated claims
        // instead of re-parsing the raw token.
        var tenantIdStr = context.User.FindFirst("tenantId")?.Value;
        if (!Guid.TryParse(tenantIdStr, out var tenantId))
        {
            logger.LogWarning("Authenticated JWT has missing or invalid tenantId claim. Path: {Path}", path);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync("""{"status":401,"message":"Tenant could not be resolved from token."}""");
            return;
        }

        tenantContext.SetTenantId(tenantId);
        await next(context);
    }
}
