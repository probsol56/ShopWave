namespace ShopWave.Application.Common.Interfaces;

public interface ITenantContext
{
    /// <summary>True once TenantResolutionMiddleware has populated the tenant.</summary>
    bool IsResolved { get; }
    Guid TenantId { get; }
}
