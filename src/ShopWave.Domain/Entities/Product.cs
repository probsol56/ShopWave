using ShopWave.Domain.Common;

namespace ShopWave.Domain.Entities;

public class Product : AuditableEntity, ITenantEntity
{
    public Guid TenantId { get; set; }
    public Guid ShopId { get; set; }
    public Guid? CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal CostPrice { get; set; }
    public int LowStockThreshold { get; set; } = 5;

    public Shop Shop { get; set; } = null!;
    public Category? Category { get; set; }
    public ICollection<StockEntry> StockEntries { get; set; } = [];
    public ICollection<SaleOrderItem> SaleOrderItems { get; set; } = [];
}
