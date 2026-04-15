using FluentValidation;
using MediatR;
using ShopWave.Application.Common.Interfaces;
using ShopWave.Application.Features.Products.DTOs;
using ShopWave.Domain.Entities;

namespace ShopWave.Application.Features.Products.Commands.CreateProduct;

public record CreateProductCommand(
    Guid ShopId,
    Guid? CategoryId,
    string Name,
    string SKU,
    decimal Price,
    decimal CostPrice,
    int LowStockThreshold = 5) : IRequest<ProductDto>;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.SKU).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CostPrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.LowStockThreshold).GreaterThanOrEqualTo(0);
    }
}

public class CreateProductCommandHandler(IAppDbContext db, ITenantContext tenantContext)
    : IRequestHandler<CreateProductCommand, ProductDto>
{
    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            TenantId = tenantContext.TenantId,
            ShopId = request.ShopId,
            CategoryId = request.CategoryId,
            Name = request.Name,
            SKU = request.SKU,
            Price = request.Price,
            CostPrice = request.CostPrice,
            LowStockThreshold = request.LowStockThreshold
        };
        db.Products.Add(product);
        await db.SaveChangesAsync(cancellationToken);

        return new ProductDto(
            product.Id, product.ShopId, product.CategoryId, product.Name, product.SKU,
            product.Price, product.CostPrice, product.LowStockThreshold, 0, product.CreatedAt);
    }
}
