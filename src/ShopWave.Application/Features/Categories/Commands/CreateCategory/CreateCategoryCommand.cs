using FluentValidation;
using MediatR;
using ShopWave.Application.Common.Interfaces;
using ShopWave.Application.Features.Categories.DTOs;
using ShopWave.Domain.Entities;

namespace ShopWave.Application.Features.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(Guid ShopId, string Name) : IRequest<CategoryDto>;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}

public class CreateCategoryCommandHandler(IAppDbContext db, ITenantContext tenantContext)
    : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new Category
        {
            TenantId = tenantContext.TenantId,
            ShopId = request.ShopId,
            Name = request.Name
        };
        db.Categories.Add(category);
        await db.SaveChangesAsync(cancellationToken);

        return new CategoryDto(category.Id, category.ShopId, category.Name, category.CreatedAt);
    }
}
