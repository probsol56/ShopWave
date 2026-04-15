using MediatR;
using Microsoft.EntityFrameworkCore;
using ShopWave.Application.Common.Interfaces;
using ShopWave.Application.Features.Dashboard.DTOs;
using ShopWave.Domain.Enums;

namespace ShopWave.Application.Features.Dashboard.Queries;

public record GetDashboardQuery(Guid ShopId) : IRequest<DashboardDto>;

public class GetDashboardQueryHandler(IAppDbContext db)
    : IRequestHandler<GetDashboardQuery, DashboardDto>
{
    public async Task<DashboardDto> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;

        var totalSalesToday = await db.SaleOrders
            .Where(o => o.ShopId == request.ShopId
                && o.CreatedAt >= today
                && o.Status != OrderStatus.Cancelled)
            .SumAsync(o => o.TotalAmount, cancellationToken);

        var totalOrdersToday = await db.SaleOrders
            .CountAsync(o => o.ShopId == request.ShopId && o.CreatedAt >= today, cancellationToken);

        var totalProducts = await db.Products
            .CountAsync(p => p.ShopId == request.ShopId, cancellationToken);

        var products = await db.Products
            .Where(p => p.ShopId == request.ShopId)
            .ToListAsync(cancellationToken);

        var productIds = products.Select(p => p.Id).ToList();
        var stockMap = await db.StockEntries
            .Where(s => productIds.Contains(s.ProductId))
            .GroupBy(s => s.ProductId)
            .Select(g => new
            {
                ProductId = g.Key,
                Stock = g.Sum(e => e.Type == StockEntryType.In ? e.Quantity :
                                   e.Type == StockEntryType.Out ? -e.Quantity : e.Quantity)
            })
            .ToDictionaryAsync(x => x.ProductId, x => x.Stock, cancellationToken);

        var lowStockCount = products.Count(p =>
            stockMap.GetValueOrDefault(p.Id, 0) <= p.LowStockThreshold);

        var recentOrders = await db.SaleOrders
            .Where(o => o.ShopId == request.ShopId)
            .OrderByDescending(o => o.CreatedAt)
            .Take(10)
            .Select(o => new RecentOrderSummary(
                o.Id, o.CustomerName, o.TotalAmount, o.Status.ToString(), o.CreatedAt))
            .ToListAsync(cancellationToken);

        return new DashboardDto(
            totalSalesToday,
            totalProducts,
            lowStockCount,
            totalOrdersToday,
            recentOrders);
    }
}
