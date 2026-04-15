namespace ShopWave.Application.Common.Interfaces;

public interface ITenantContext
{
    /// <summary>True once TenantResolutionMiddleware has populated the tenant.</summary>
    bool IsResolved { get; }

    /// <summary>Returns the tenant ID. Throws if not resolved — use only in authenticated contexts.</summary>
    Guid TenantId { get; }

    /// <summary>Returns the tenant ID or Guid.Empty if not resolved. Safe for use in EF query filters.</summary>
    Guid ResolvedTenantId { get; }
}
