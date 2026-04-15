using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ShopWave.Application.Common.Exceptions;
using ShopWave.Application.Common.Interfaces;
using ShopWave.Application.Features.Tenant.DTOs;

namespace ShopWave.Application.Features.Tenant.Commands;

public record UpdateTenantCommand(string Name) : IRequest<TenantDto>;

public class UpdateTenantCommandValidator : AbstractValidator<UpdateTenantCommand>
{
    public UpdateTenantCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}

public class UpdateTenantCommandHandler(IAppDbContext db, ITenantContext tenantContext)
    : IRequestHandler<UpdateTenantCommand, TenantDto>
{
    public async Task<TenantDto> Handle(UpdateTenantCommand request, CancellationToken cancellationToken)
    {
        var tenant = await db.Tenants
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.Id == tenantContext.TenantId, cancellationToken)
            ?? throw new NotFoundException("Tenant", tenantContext.TenantId);

        tenant.Name = request.Name;
        tenant.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(cancellationToken);

        return new TenantDto(tenant.Id, tenant.Name, tenant.Slug, tenant.IsActive, tenant.CreatedAt);
    }
}
