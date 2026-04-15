using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopWave.Application.Features.Dashboard.Queries;

namespace ShopWave.API.Controllers;

[Authorize]
[Route("api/shops/{shopId:guid}/dashboard")]
[ApiController]
public class DashboardController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> Get(Guid shopId, CancellationToken ct)
        => Ok(await Mediator.Send(new GetDashboardQuery(shopId), ct));
}
