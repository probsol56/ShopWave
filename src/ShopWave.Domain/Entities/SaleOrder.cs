using ShopWave.Domain.Common;
using ShopWave.Domain.Enums;

namespace ShopWave.Domain.Entities;

public class SaleOrder : AuditableEntity, ITenantEntity
{
    public Guid TenantId { get; set; }
    public Guid ShopId { get; set; }
    public string? CustomerName { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public Shop Shop { get; set; } = null!;
    public ICollection<SaleOrderItem> Items { get; set; } = [];
}
