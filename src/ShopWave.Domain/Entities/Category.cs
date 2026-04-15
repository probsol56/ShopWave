using ShopWave.Domain.Common;

namespace ShopWave.Domain.Entities;

public class Category : AuditableEntity, ITenantEntity
{
    public Guid TenantId { get; set; }
    public Guid ShopId { get; set; }
    public string Name { get; set; } = string.Empty;

    public Shop Shop { get; set; } = null!;
    public ICollection<Product> Products { get; set; } = [];
}
