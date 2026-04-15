using ShopWave.Domain.Common;
using ShopWave.Domain.Enums;

namespace ShopWave.Domain.Entities;

public class StockEntry : AuditableEntity, ITenantEntity
{
    public Guid TenantId { get; set; }
    public Guid ShopId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public StockEntryType Type { get; set; }
    public string? Note { get; set; }

    public Shop Shop { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
