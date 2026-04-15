using ShopWave.Domain.Common;

namespace ShopWave.Domain.Entities;

public class Tenant : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public ICollection<User> Users { get; set; } = [];
    public ICollection<Shop> Shops { get; set; } = [];
}
