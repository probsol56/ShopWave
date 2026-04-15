using MediatR;
using Microsoft.EntityFrameworkCore;
using ShopWave.Application.Common.Exceptions;
using ShopWave.Application.Common.Interfaces;
using ShopWave.Application.Features.Shops.DTOs;

namespace ShopWave.Application.Features.Shops.Queries;

public record GetShopByIdQuery(Guid ShopId) : IRequest<ShopDto>;

public class GetShopByIdQueryHandler(IAppDbContext db)
    : IRequestHandler<GetShopByIdQuery, ShopDto>
{
    public async Task<ShopDto> Handle(GetShopByIdQuery request, CancellationToken cancellationToken)
    {
        var shop = await db.Shops
            .FirstOrDefaultAsync(s => s.Id == request.ShopId, cancellationToken)
            ?? throw new NotFoundException("Shop", request.ShopId);

        return new ShopDto(shop.Id, shop.Name, shop.Address, shop.Phone, shop.Currency, shop.CreatedAt);
    }
}
