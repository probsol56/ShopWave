using MediatR;
using Microsoft.EntityFrameworkCore;
using ShopWave.Application.Common.Interfaces;
using ShopWave.Application.Features.Stock.DTOs;
using ShopWave.Domain.Enums;

namespace ShopWave.Application.Features.Stock.Queries;

public record GetLowStockQuery(Guid ShopId) : IRequest<List<StockLevelDto>>;

public class GetLowStockQueryHandler(IAppDbContext db)
    : IRequestHandler<GetLowStockQuery, List<StockLevelDto>>
{
    public async Task<List<StockLevelDto>> Handle(GetLowStockQuery request, CancellationToken cancellationToken)
    {
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

        return products
            .Select(p =>
            {
                var stock = stockMap.GetValueOrDefault(p.Id, 0);
                return new StockLevelDto(p.Id, p.Name, p.SKU, stock, p.LowStockThreshold, stock <= p.LowStockThreshold);
            })
            .Where(s => s.IsLowStock)
            .ToList();
    }
}
