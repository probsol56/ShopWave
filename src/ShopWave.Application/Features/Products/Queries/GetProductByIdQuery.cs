using MediatR;
using Microsoft.EntityFrameworkCore;
using ShopWave.Application.Common.Exceptions;
using ShopWave.Application.Common.Interfaces;
using ShopWave.Application.Features.Products.DTOs;
using ShopWave.Domain.Enums;

namespace ShopWave.Application.Features.Products.Queries;

public record GetProductByIdQuery(Guid ShopId, Guid ProductId) : IRequest<ProductDto>;

public class GetProductByIdQueryHandler(IAppDbContext db)
    : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await db.Products
            .FirstOrDefaultAsync(p => p.Id == request.ProductId && p.ShopId == request.ShopId, cancellationToken)
            ?? throw new NotFoundException("Product", request.ProductId);

        var stock = await db.StockEntries
            .Where(s => s.ProductId == product.Id)
            .SumAsync(s => s.Type == StockEntryType.In ? s.Quantity :
                           s.Type == StockEntryType.Out ? -s.Quantity : s.Quantity, cancellationToken);

        return new ProductDto(
            product.Id, product.ShopId, product.CategoryId, product.Name, product.SKU,
            product.Price, product.CostPrice, product.LowStockThreshold, stock, product.CreatedAt);
    }
}
