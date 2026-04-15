using MediatR;
using Microsoft.EntityFrameworkCore;
using ShopWave.Application.Common.Interfaces;
using ShopWave.Application.Features.Shops.DTOs;

namespace ShopWave.Application.Features.Shops.Queries;

public record GetShopsQuery : IRequest<List<ShopDto>>;

public class GetShopsQueryHandler(IAppDbContext db)
    : IRequestHandler<GetShopsQuery, List<ShopDto>>
{
    public async Task<List<ShopDto>> Handle(GetShopsQuery request, CancellationToken cancellationToken)
    {
        return await db.Shops
            .OrderBy(s => s.Name)
            .Select(s => new ShopDto(s.Id, s.Name, s.Address, s.Phone, s.Currency, s.CreatedAt))
            .ToListAsync(cancellationToken);
    }
}
