namespace ShopWave.Application.Features.Categories.DTOs;

public record CategoryDto(Guid Id, Guid ShopId, string Name, DateTime CreatedAt);
