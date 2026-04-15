using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopWave.Application.Features.Shops.Commands.CreateShop;
using ShopWave.Application.Features.Shops.Commands.DeleteShop;
using ShopWave.Application.Features.Shops.Commands.UpdateShop;
using ShopWave.Application.Features.Shops.Queries;

namespace ShopWave.API.Controllers;

[Authorize]
public class ShopsController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await Mediator.Send(new GetShopsQuery(), ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await Mediator.Send(new GetShopByIdQuery(id), ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateShopCommand command, CancellationToken ct)
    {
        var result = await Mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateShopRequest request, CancellationToken ct)
        => Ok(await Mediator.Send(new UpdateShopCommand(id, request.Name, request.Address, request.Phone, request.Currency), ct));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await Mediator.Send(new DeleteShopCommand(id), ct);
        return NoContent();
    }
}

public record UpdateShopRequest(string Name, string? Address, string? Phone, string Currency);
