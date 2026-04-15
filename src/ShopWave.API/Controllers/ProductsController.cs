using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopWave.Application.Features.Products.Commands.CreateProduct;
using ShopWave.Application.Features.Products.Commands.DeleteProduct;
using ShopWave.Application.Features.Products.Commands.UpdateProduct;
using ShopWave.Application.Features.Products.Queries;

namespace ShopWave.API.Controllers;

[Authorize]
[Route("api/shops/{shopId:guid}/products")]
[ApiController]
public class ProductsController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(Guid shopId, CancellationToken ct)
        => Ok(await Mediator.Send(new GetProductsQuery(shopId), ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid shopId, Guid id, CancellationToken ct)
        => Ok(await Mediator.Send(new GetProductByIdQuery(shopId, id), ct));

    [HttpPost]
    public async Task<IActionResult> Create(Guid shopId, [FromBody] CreateProductCommand command, CancellationToken ct)
    {
        var result = await Mediator.Send(command with { ShopId = shopId }, ct);
        return CreatedAtAction(nameof(GetById), new { shopId, id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid shopId, Guid id, [FromBody] UpdateProductRequest request, CancellationToken ct)
        => Ok(await Mediator.Send(
            new UpdateProductCommand(shopId, id, request.CategoryId, request.Name, request.SKU,
                request.Price, request.CostPrice, request.LowStockThreshold), ct));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid shopId, Guid id, CancellationToken ct)
    {
        await Mediator.Send(new DeleteProductCommand(shopId, id), ct);
        return NoContent();
    }
}

public record UpdateProductRequest(
    Guid? CategoryId,
    string Name,
    string SKU,
    decimal Price,
    decimal CostPrice,
    int LowStockThreshold);
