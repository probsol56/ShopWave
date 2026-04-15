using MediatR;
using Microsoft.EntityFrameworkCore;
using ShopWave.Application.Common.Interfaces;
using ShopWave.Application.Features.Products.DTOs;
using ShopWave.Domain.Enums;

namespace ShopWave.Application.Features.Products.Queries;

public record GetProductsQuery(Guid ShopId) : IRequest<List<ProductDto>>;

public class GetProductsQueryHandler(IAppDbContext db)
    : IRequestHandler<GetProductsQuery, List<ProductDto>>
{
    public async Task<List<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await db.Products
            .Where(p => p.ShopId == request.ShopId)
            .OrderBy(p => p.Name)
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

        return products.Select(p => new ProductDto(
            p.Id, p.ShopId, p.CategoryId, p.Name, p.SKU,
            p.Price, p.CostPrice, p.LowStockThreshold,
            stockMap.GetValueOrDefault(p.Id, 0),
            p.CreatedAt)).ToList();
    }
}
