using ShopWave.Application.Common.Interfaces;

namespace ShopWave.Infrastructure.Services;

public class TenantContext : ITenantContext
{
    private Guid _tenantId;

    public bool IsResolved => _tenantId != Guid.Empty;

    public Guid TenantId
    {
        get => _tenantId == Guid.Empty
            ? throw new InvalidOperationException("TenantId has not been set. Ensure TenantResolutionMiddleware runs first.")
            : _tenantId;
    }

    public void SetTenantId(Guid tenantId) => _tenantId = tenantId;
}
