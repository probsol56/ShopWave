using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopWave.Application.Features.Stock.Commands.AdjustStock;
using ShopWave.Application.Features.Stock.Queries;
using ShopWave.Domain.Enums;

namespace ShopWave.API.Controllers;

[Authorize]
[Route("api/shops/{shopId:guid}/stock")]
[ApiController]
public class StockController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetStock(Guid shopId, CancellationToken ct)
        => Ok(await Mediator.Send(new GetStockQuery(shopId), ct));

    [HttpGet("low-stock")]
    public async Task<IActionResult> GetLowStock(Guid shopId, CancellationToken ct)
        => Ok(await Mediator.Send(new GetLowStockQuery(shopId), ct));

    [HttpPost("adjust")]
    public async Task<IActionResult> Adjust(Guid shopId, [FromBody] AdjustStockRequest request, CancellationToken ct)
    {
        var result = await Mediator.Send(
            new AdjustStockCommand(shopId, request.ProductId, request.Quantity, request.Type, request.Note), ct);
        return Ok(result);
    }
}

public record AdjustStockRequest(Guid ProductId, int Quantity, StockEntryType Type, string? Note);
