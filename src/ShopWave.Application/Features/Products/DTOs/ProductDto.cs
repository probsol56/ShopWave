namespace ShopWave.Application.Features.Products.DTOs;

public record ProductDto(
    Guid Id,
    Guid ShopId,
    Guid? CategoryId,
    string Name,
    string SKU,
    decimal Price,
    decimal CostPrice,
    int LowStockThreshold,
    int CurrentStock,
    DateTime CreatedAt);
