using FluentValidation;
using MediatR;
using ShopWave.Application.Common.Interfaces;
using ShopWave.Application.Features.Shops.DTOs;
using ShopWave.Domain.Entities;

namespace ShopWave.Application.Features.Shops.Commands.CreateShop;

public record CreateShopCommand(string Name, string? Address, string? Phone, string Currency = "USD")
    : IRequest<ShopDto>;

public class CreateShopCommandValidator : AbstractValidator<CreateShopCommand>
{
    public CreateShopCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Currency).NotEmpty().Length(3).WithMessage("Currency must be a 3-letter code.");
    }
}

public class CreateShopCommandHandler(IAppDbContext db, ITenantContext tenantContext)
    : IRequestHandler<CreateShopCommand, ShopDto>
{
    public async Task<ShopDto> Handle(CreateShopCommand request, CancellationToken cancellationToken)
    {
        var shop = new Shop
        {
            TenantId = tenantContext.TenantId,
            Name = request.Name,
            Address = request.Address,
            Phone = request.Phone,
            Currency = request.Currency
        };
        db.Shops.Add(shop);
        await db.SaveChangesAsync(cancellationToken);

        return new ShopDto(shop.Id, shop.Name, shop.Address, shop.Phone, shop.Currency, shop.CreatedAt);
    }
}
