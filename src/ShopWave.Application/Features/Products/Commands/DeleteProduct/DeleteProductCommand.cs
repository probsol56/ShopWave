using MediatR;
using Microsoft.EntityFrameworkCore;
using ShopWave.Application.Common.Exceptions;
using ShopWave.Application.Common.Interfaces;

namespace ShopWave.Application.Features.Products.Commands.DeleteProduct;

public record DeleteProductCommand(Guid ShopId, Guid ProductId) : IRequest;

public class DeleteProductCommandHandler(IAppDbContext db)
    : IRequestHandler<DeleteProductCommand>
{
    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await db.Products
            .FirstOrDefaultAsync(p => p.Id == request.ProductId && p.ShopId == request.ShopId, cancellationToken)
            ?? throw new NotFoundException("Product", request.ProductId);

        db.Products.Remove(product);
        await db.SaveChangesAsync(cancellationToken);
    }
}
