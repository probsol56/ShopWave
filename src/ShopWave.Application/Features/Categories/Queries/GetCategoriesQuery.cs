using MediatR;
using Microsoft.EntityFrameworkCore;
using ShopWave.Application.Common.Interfaces;
using ShopWave.Application.Features.Categories.DTOs;

namespace ShopWave.Application.Features.Categories.Queries;

public record GetCategoriesQuery(Guid ShopId) : IRequest<List<CategoryDto>>;

public class GetCategoriesQueryHandler(IAppDbContext db)
    : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
{
    public async Task<List<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        return await db.Categories
            .Where(c => c.ShopId == request.ShopId)
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto(c.Id, c.ShopId, c.Name, c.CreatedAt))
            .ToListAsync(cancellationToken);
    }
}
