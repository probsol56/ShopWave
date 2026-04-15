using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ShopWave.Application.Common.Exceptions;
using ShopWave.Application.Common.Interfaces;
using ShopWave.Application.Features.SaleOrders.DTOs;
using ShopWave.Domain.Enums;

namespace ShopWave.Application.Features.SaleOrders.Commands.UpdateOrderStatus;

public record UpdateOrderStatusCommand(Guid ShopId, Guid OrderId, OrderStatus Status) : IRequest<SaleOrderDto>;

public class UpdateOrderStatusCommandValidator : AbstractValidator<UpdateOrderStatusCommand>
{
    public UpdateOrderStatusCommandValidator()
    {
        RuleFor(x => x.Status).IsInEnum();
    }
}

public class UpdateOrderStatusCommandHandler(IAppDbContext db)
    : IRequestHandler<UpdateOrderStatusCommand, SaleOrderDto>
{
    public async Task<SaleOrderDto> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await db.SaleOrders
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId && o.ShopId == request.ShopId, cancellationToken)
            ?? throw new NotFoundException("SaleOrder", request.OrderId);

        order.Status = request.Status;
        order.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(cancellationToken);

        return new SaleOrderDto(
            order.Id, order.ShopId, order.CustomerName, order.TotalAmount, order.Status.ToString(),
            order.Items.Select(i => new SaleOrderItemDto(
                i.Id, i.ProductId, i.Product.Name, i.Quantity, i.UnitPrice, i.Quantity * i.UnitPrice)).ToList(),
            order.CreatedAt);
    }
}
