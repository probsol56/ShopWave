namespace ShopWave.Application.Features.Stock.DTOs;

public record StockLevelDto(
    Guid ProductId,
    string ProductName,
    string SKU,
    int CurrentStock,
    int LowStockThreshold,
    bool IsLowStock);

public record StockEntryDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    int Quantity,
    string Type,
    string? Note,
    DateTime CreatedAt);
