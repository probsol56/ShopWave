using MediatR;
using Microsoft.EntityFrameworkCore;
using ShopWave.Application.Common.Exceptions;
using ShopWave.Application.Common.Interfaces;

namespace ShopWave.Application.Features.Shops.Commands.DeleteShop;

public record DeleteShopCommand(Guid ShopId) : IRequest;

public class DeleteShopCommandHandler(IAppDbContext db)
    : IRequestHandler<DeleteShopCommand>
{
    public async Task Handle(DeleteShopCommand request, CancellationToken cancellationToken)
    {
        var shop = await db.Shops
            .FirstOrDefaultAsync(s => s.Id == request.ShopId, cancellationToken)
            ?? throw new NotFoundException("Shop", request.ShopId);

        db.Shops.Remove(shop);
        await db.SaveChangesAsync(cancellationToken);
    }
}
