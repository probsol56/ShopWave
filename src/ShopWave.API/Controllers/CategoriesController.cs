using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopWave.Application.Features.Categories.Commands.CreateCategory;
using ShopWave.Application.Features.Categories.Commands.UpdateCategory;
using ShopWave.Application.Features.Categories.Queries;

namespace ShopWave.API.Controllers;

[Authorize]
[Route("api/shops/{shopId:guid}/categories")]
[ApiController]
public class CategoriesController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(Guid shopId, CancellationToken ct)
        => Ok(await Mediator.Send(new GetCategoriesQuery(shopId), ct));

    [HttpPost]
    public async Task<IActionResult> Create(Guid shopId, [FromBody] CreateCategoryRequest request, CancellationToken ct)
    {
        var result = await Mediator.Send(new CreateCategoryCommand(shopId, request.Name), ct);
        return CreatedAtAction(nameof(GetAll), new { shopId }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid shopId, Guid id, [FromBody] UpdateCategoryRequest request, CancellationToken ct)
        => Ok(await Mediator.Send(new UpdateCategoryCommand(shopId, id, request.Name), ct));
}

public record CreateCategoryRequest(string Name);
public record UpdateCategoryRequest(string Name);
