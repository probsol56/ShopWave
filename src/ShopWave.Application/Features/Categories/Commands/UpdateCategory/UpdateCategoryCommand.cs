using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ShopWave.Application.Common.Exceptions;
using ShopWave.Application.Common.Interfaces;
using ShopWave.Application.Features.Categories.DTOs;

namespace ShopWave.Application.Features.Categories.Commands.UpdateCategory;

public record UpdateCategoryCommand(Guid ShopId, Guid CategoryId, string Name) : IRequest<CategoryDto>;

public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}

public class UpdateCategoryCommandHandler(IAppDbContext db)
    : IRequestHandler<UpdateCategoryCommand, CategoryDto>
{
    public async Task<CategoryDto> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await db.Categories
            .FirstOrDefaultAsync(c => c.Id == request.CategoryId && c.ShopId == request.ShopId, cancellationToken)
            ?? throw new NotFoundException("Category", request.CategoryId);

        category.Name = request.Name;
        category.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(cancellationToken);

        return new CategoryDto(category.Id, category.ShopId, category.Name, category.CreatedAt);
    }
}
