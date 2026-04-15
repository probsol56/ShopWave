using ShopWave.Domain.Common;
using ShopWave.Domain.Enums;

namespace ShopWave.Domain.Entities;

public class User : AuditableEntity, ITenantEntity
{
    public Guid TenantId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }

    public Tenant Tenant { get; set; } = null!;
}
