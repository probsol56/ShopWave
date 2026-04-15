using ShopWave.Domain.Enums;

namespace ShopWave.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid UserId { get; }
    Guid TenantId { get; }
    string Email { get; }
    UserRole Role { get; }
    bool IsAuthenticated { get; }
}
