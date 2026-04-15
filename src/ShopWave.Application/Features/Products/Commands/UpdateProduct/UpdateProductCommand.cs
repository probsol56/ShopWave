using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ShopWave.Application.Common.Exceptions;
using ShopWave.Application.Common.Interfaces;
using ShopWave.Application.Features.Products.DTOs;
using ShopWave.Domain.Enums;

namespace ShopWave.Application.Features.Products.Commands.UpdateProduct;

public record UpdateProductCommand(
    Guid ShopId,
    Guid ProductId,
    Guid? CategoryId,
    string Name,
    string SKU,
    decimal Price,
    decimal CostPrice,
    int LowStockThreshold) : IRequest<ProductDto>;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.SKU).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CostPrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.LowStockThreshold).GreaterThanOrEqualTo(0);
    }
}

public class UpdateProductCommandHandler(IAppDbContext db)
    : IRequestHandler<UpdateProductCommand, ProductDto>
{
    public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await db.Products
            .FirstOrDefaultAsync(p => p.Id == request.ProductId && p.ShopId == request.ShopId, cancellationToken)
            ?? throw new NotFoundException("Product", request.ProductId);

        product.CategoryId = request.CategoryId;
        product.Name = request.Name;
        product.SKU = request.SKU;
        product.Price = request.Price;
        product.CostPrice = request.CostPrice;
        product.LowStockThreshold = request.LowStockThreshold;
        product.UpdatedAt = DateTime.UtcNow;

        var stock = await db.StockEntries
            .Where(s => s.ProductId == product.Id)
            .SumAsync(s => s.Type == StockEntryType.In ? s.Quantity :
                           s.Type == StockEntryType.Out ? -s.Quantity : s.Quantity, cancellationToken);

        await db.SaveChangesAsync(cancellationToken);

        return new ProductDto(
            product.Id, product.ShopId, product.CategoryId, product.Name, product.SKU,
            product.Price, product.CostPrice, product.LowStockThreshold, stock, product.CreatedAt);
    }
}
