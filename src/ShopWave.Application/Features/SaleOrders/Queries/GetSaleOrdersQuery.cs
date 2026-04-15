using MediatR;
using Microsoft.EntityFrameworkCore;
using ShopWave.Application.Common.Interfaces;
using ShopWave.Application.Features.SaleOrders.DTOs;

namespace ShopWave.Application.Features.SaleOrders.Queries;

public record GetSaleOrdersQuery(Guid ShopId) : IRequest<List<SaleOrderDto>>;

public class GetSaleOrdersQueryHandler(IAppDbContext db)
    : IRequestHandler<GetSaleOrdersQuery, List<SaleOrderDto>>
{
    public async Task<List<SaleOrderDto>> Handle(GetSaleOrdersQuery request, CancellationToken cancellationToken)
    {
        return await db.SaleOrders
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .Where(o => o.ShopId == request.ShopId)
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new SaleOrderDto(
                o.Id, o.ShopId, o.CustomerName, o.TotalAmount, o.Status.ToString(),
                o.Items.Select(i => new SaleOrderItemDto(
                    i.Id, i.ProductId, i.Product.Name, i.Quantity, i.UnitPrice, i.Quantity * i.UnitPrice)).ToList(),
                o.CreatedAt))
            .ToListAsync(cancellationToken);
    }
}
