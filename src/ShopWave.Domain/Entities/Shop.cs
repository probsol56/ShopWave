using ShopWave.Domain.Common;

namespace ShopWave.Domain.Entities;

public class Shop : AuditableEntity, ITenantEntity
{
    public Guid TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string Currency { get; set; } = "USD";

    public Tenant Tenant { get; set; } = null!;
    public ICollection<Category> Categories { get; set; } = [];
    public ICollection<Product> Products { get; set; } = [];
    public ICollection<StockEntry> StockEntries { get; set; } = [];
    public ICollection<SaleOrder> SaleOrders { get; set; } = [];
}
