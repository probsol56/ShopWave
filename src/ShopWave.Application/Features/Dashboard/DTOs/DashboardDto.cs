namespace ShopWave.Application.Features.Dashboard.DTOs;

public record RecentOrderSummary(Guid Id, string? CustomerName, decimal TotalAmount, string Status, DateTime CreatedAt);

public record DashboardDto(
    decimal TotalSalesToday,
    int TotalProducts,
    int LowStockCount,
    int TotalOrdersToday,
    List<RecentOrderSummary> RecentOrders);
