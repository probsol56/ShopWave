using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopWave.Application.Features.Tenant.Commands;
using ShopWave.Application.Features.Tenant.Queries;

namespace ShopWave.API.Controllers;

[Authorize]
public class TenantController : BaseController
{
    [HttpGet("me")]
    public async Task<IActionResult> GetMyTenant(CancellationToken ct)
        => Ok(await Mediator.Send(new GetMyTenantQuery(), ct));

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMyTenant([FromBody] UpdateTenantCommand command, CancellationToken ct)
        => Ok(await Mediator.Send(command, ct));
}
