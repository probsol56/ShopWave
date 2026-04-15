using MediatR;
using Microsoft.EntityFrameworkCore;
using ShopWave.Application.Common.Exceptions;
using ShopWave.Application.Common.Interfaces;
using ShopWave.Application.Features.SaleOrders.DTOs;

namespace ShopWave.Application.Features.SaleOrders.Queries;

public record GetSaleOrderByIdQuery(Guid ShopId, Guid OrderId) : IRequest<SaleOrderDto>;

public class GetSaleOrderByIdQueryHandler(IAppDbContext db)
    : IRequestHandler<GetSaleOrderByIdQuery, SaleOrderDto>
{
    public async Task<SaleOrderDto> Handle(GetSaleOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await db.SaleOrders
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId && o.ShopId == request.ShopId, cancellationToken)
            ?? throw new NotFoundException("SaleOrder", request.OrderId);

        return new SaleOrderDto(
            order.Id, order.ShopId, order.CustomerName, order.TotalAmount, order.Status.ToString(),
            order.Items.Select(i => new SaleOrderItemDto(
                i.Id, i.ProductId, i.Product.Name, i.Quantity, i.UnitPrice, i.Quantity * i.UnitPrice)).ToList(),
            order.CreatedAt);
    }
}
