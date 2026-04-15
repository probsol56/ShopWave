using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ShopWave.Application.Common.Exceptions;
using ShopWave.Application.Common.Interfaces;
using ShopWave.Application.Features.Stock.DTOs;
using ShopWave.Domain.Entities;
using ShopWave.Domain.Enums;

namespace ShopWave.Application.Features.Stock.Commands.AdjustStock;

public record AdjustStockCommand(
    Guid ShopId,
    Guid ProductId,
    int Quantity,
    StockEntryType Type,
    string? Note) : IRequest<StockEntryDto>;

public class AdjustStockCommandValidator : AbstractValidator<AdjustStockCommand>
{
    public AdjustStockCommandValidator()
    {
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than 0.");
        RuleFor(x => x.Type).IsInEnum();
    }
}

public class AdjustStockCommandHandler(IAppDbContext db, ITenantContext tenantContext)
    : IRequestHandler<AdjustStockCommand, StockEntryDto>
{
    public async Task<StockEntryDto> Handle(AdjustStockCommand request, CancellationToken cancellationToken)
    {
        var product = await db.Products
            .FirstOrDefaultAsync(p => p.Id == request.ProductId && p.ShopId == request.ShopId, cancellationToken)
            ?? throw new NotFoundException("Product", request.ProductId);

        var entry = new StockEntry
        {
            TenantId = tenantContext.TenantId,
            ShopId = request.ShopId,
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            Type = request.Type,
            Note = request.Note
        };
        db.StockEntries.Add(entry);
        await db.SaveChangesAsync(cancellationToken);

        return new StockEntryDto(
            entry.Id, entry.ProductId, product.Name,
            entry.Quantity, entry.Type.ToString(), entry.Note, entry.CreatedAt);
    }
}
