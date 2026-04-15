using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopWave.Application.Features.SaleOrders.Commands.CreateSaleOrder;
using ShopWave.Application.Features.SaleOrders.Commands.UpdateOrderStatus;
using ShopWave.Application.Features.SaleOrders.DTOs;
using ShopWave.Application.Features.SaleOrders.Queries;
using ShopWave.Domain.Enums;

namespace ShopWave.API.Controllers;

[Authorize]
[Route("api/shops/{shopId:guid}/sale-orders")]
[ApiController]
public class SaleOrdersController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(Guid shopId, CancellationToken ct)
        => Ok(await Mediator.Send(new GetSaleOrdersQuery(shopId), ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid shopId, Guid id, CancellationToken ct)
        => Ok(await Mediator.Send(new GetSaleOrderByIdQuery(shopId, id), ct));

    [HttpPost]
    public async Task<IActionResult> Create(Guid shopId, [FromBody] CreateSaleOrderRequest request, CancellationToken ct)
    {
        var result = await Mediator.Send(
            new CreateSaleOrderCommand(shopId, request.CustomerName, request.Items), ct);
        return CreatedAtAction(nameof(GetById), new { shopId, id = result.Id }, result);
    }

    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid shopId, Guid id, [FromBody] UpdateOrderStatusRequest request, CancellationToken ct)
        => Ok(await Mediator.Send(new UpdateOrderStatusCommand(shopId, id, request.Status), ct));
}

public record CreateSaleOrderRequest(string? CustomerName, List<CreateSaleOrderItemRequest> Items);
public record UpdateOrderStatusRequest(OrderStatus Status);
