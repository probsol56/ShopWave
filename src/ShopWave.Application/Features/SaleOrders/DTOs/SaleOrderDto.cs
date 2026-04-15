namespace ShopWave.Application.Features.SaleOrders.DTOs;

public record SaleOrderItemDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal LineTotal);

public record SaleOrderDto(
    Guid Id,
    Guid ShopId,
    string? CustomerName,
    decimal TotalAmount,
    string Status,
    List<SaleOrderItemDto> Items,
    DateTime CreatedAt);

public record CreateSaleOrderItemRequest(Guid ProductId, int Quantity, decimal UnitPrice);
