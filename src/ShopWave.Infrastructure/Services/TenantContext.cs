using ShopWave.Application.Common.Interfaces;

namespace ShopWave.Infrastructure.Services;

public class TenantContext : ITenantContext
{
    private Guid _tenantId;

    public bool IsResolved => _tenantId != Guid.Empty;

    /// <inheritdoc/>
    public Guid TenantId
    {
        get => _tenantId == Guid.Empty
            ? throw new InvalidOperationException("TenantId has not been set. Ensure TenantResolutionMiddleware runs first.")
            : _tenantId;
    }

    /// <inheritdoc/>
    /// Never throws — returns Guid.Empty when unresolved. Safe for EF Core query filters.
    public Guid ResolvedTenantId => _tenantId;

    public void SetTenantId(Guid tenantId) => _tenantId = tenantId;
}
