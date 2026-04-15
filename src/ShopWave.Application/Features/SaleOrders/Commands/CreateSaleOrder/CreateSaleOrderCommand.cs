using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ShopWave.Application.Common.Exceptions;
using ShopWave.Application.Common.Interfaces;
using ShopWave.Application.Features.SaleOrders.DTOs;
using ShopWave.Domain.Entities;
using ShopWave.Domain.Enums;

namespace ShopWave.Application.Features.SaleOrders.Commands.CreateSaleOrder;

public record CreateSaleOrderCommand(
    Guid ShopId,
    string? CustomerName,
    List<CreateSaleOrderItemRequest> Items) : IRequest<SaleOrderDto>;

public class CreateSaleOrderCommandValidator : AbstractValidator<CreateSaleOrderCommand>
{
    public CreateSaleOrderCommandValidator()
    {
        RuleFor(x => x.Items).NotEmpty().WithMessage("Sale order must have at least one item.");
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.Quantity).GreaterThan(0);
            item.RuleFor(i => i.UnitPrice).GreaterThanOrEqualTo(0);
        });
    }
}

public class CreateSaleOrderCommandHandler(IAppDbContext db, ITenantContext tenantContext)
    : IRequestHandler<CreateSaleOrderCommand, SaleOrderDto>
{
    public async Task<SaleOrderDto> Handle(CreateSaleOrderCommand request, CancellationToken cancellationToken)
    {
        var productIds = request.Items.Select(i => i.ProductId).ToList();
        var products = await db.Products
            .Where(p => productIds.Contains(p.Id) && p.ShopId == request.ShopId)
            .ToDictionaryAsync(p => p.Id, cancellationToken);

        foreach (var item in request.Items)
        {
            if (!products.ContainsKey(item.ProductId))
                throw new NotFoundException("Product", item.ProductId);
        }

        var order = new SaleOrder
        {
            TenantId = tenantContext.TenantId,
            ShopId = request.ShopId,
            CustomerName = request.CustomerName,
            Status = OrderStatus.Pending
        };

        var orderItems = request.Items.Select(i => new SaleOrderItem
        {
            SaleOrderId = order.Id,
            ProductId = i.ProductId,
            Quantity = i.Quantity,
            UnitPrice = i.UnitPrice
        }).ToList();

        order.TotalAmount = orderItems.Sum(i => i.Quantity * i.UnitPrice);
        order.Items = orderItems;

        // Create stock-out entries for each sold item
        foreach (var item in orderItems)
        {
            db.StockEntries.Add(new StockEntry
            {
                TenantId = tenantContext.TenantId,
                ShopId = request.ShopId,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Type = StockEntryType.Out,
                Note = $"Sale Order {order.Id}"
            });
        }

        db.SaleOrders.Add(order);
        await db.SaveChangesAsync(cancellationToken);

        return new SaleOrderDto(
            order.Id, order.ShopId, order.CustomerName, order.TotalAmount, order.Status.ToString(),
            orderItems.Select(i => new SaleOrderItemDto(
                i.Id, i.ProductId, products[i.ProductId].Name, i.Quantity, i.UnitPrice, i.Quantity * i.UnitPrice)).ToList(),
            order.CreatedAt);
    }
}
