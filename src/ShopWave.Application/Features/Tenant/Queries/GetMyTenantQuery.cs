using MediatR;
using Microsoft.EntityFrameworkCore;
using ShopWave.Application.Common.Exceptions;
using ShopWave.Application.Common.Interfaces;
using ShopWave.Application.Features.Tenant.DTOs;

namespace ShopWave.Application.Features.Tenant.Queries;

public record GetMyTenantQuery : IRequest<TenantDto>;

public class GetMyTenantQueryHandler(IAppDbContext db, ITenantContext tenantContext)
    : IRequestHandler<GetMyTenantQuery, TenantDto>
{
    public async Task<TenantDto> Handle(GetMyTenantQuery request, CancellationToken cancellationToken)
    {
        var tenant = await db.Tenants
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.Id == tenantContext.TenantId, cancellationToken)
            ?? throw new NotFoundException("Tenant", tenantContext.TenantId);

        return new TenantDto(tenant.Id, tenant.Name, tenant.Slug, tenant.IsActive, tenant.CreatedAt);
    }
}
