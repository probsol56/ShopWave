namespace ShopWave.Application.Features.Shops.DTOs;

public record ShopDto(
    Guid Id,
    string Name,
    string? Address,
    string? Phone,
    string Currency,
    DateTime CreatedAt);
