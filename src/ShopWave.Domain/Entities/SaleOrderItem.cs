using ShopWave.Domain.Common;

namespace ShopWave.Domain.Entities;

public class SaleOrderItem : BaseEntity
{
    public Guid SaleOrderId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    public SaleOrder SaleOrder { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
