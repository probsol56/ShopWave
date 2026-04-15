using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ShopWave.Application.Common.Exceptions;
using ShopWave.Application.Common.Interfaces;
using ShopWave.Application.Features.Shops.DTOs;

namespace ShopWave.Application.Features.Shops.Commands.UpdateShop;

public record UpdateShopCommand(Guid ShopId, string Name, string? Address, string? Phone, string Currency)
    : IRequest<ShopDto>;

public class UpdateShopCommandValidator : AbstractValidator<UpdateShopCommand>
{
    public UpdateShopCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Currency).NotEmpty().Length(3);
    }
}

public class UpdateShopCommandHandler(IAppDbContext db)
    : IRequestHandler<UpdateShopCommand, ShopDto>
{
    public async Task<ShopDto> Handle(UpdateShopCommand request, CancellationToken cancellationToken)
    {
        var shop = await db.Shops
            .FirstOrDefaultAsync(s => s.Id == request.ShopId, cancellationToken)
            ?? throw new NotFoundException("Shop", request.ShopId);

        shop.Name = request.Name;
        shop.Address = request.Address;
        shop.Phone = request.Phone;
        shop.Currency = request.Currency;
        shop.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(cancellationToken);

        return new ShopDto(shop.Id, shop.Name, shop.Address, shop.Phone, shop.Currency, shop.CreatedAt);
    }
}
