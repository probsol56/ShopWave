namespace ShopWave.Application.Features.Tenant.DTOs;

public record TenantDto(
    Guid Id,
    string Name,
    string Slug,
    bool IsActive,
    DateTime CreatedAt);
