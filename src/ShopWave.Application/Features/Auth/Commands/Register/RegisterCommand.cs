using MediatR;
using ShopWave.Application.Features.Auth.DTOs;

namespace ShopWave.Application.Features.Auth.Commands.Register;

public record RegisterCommand(
    string TenantName,
    string TenantSlug,
    string Email,
    string Password,
    string FirstName,
    string LastName) : IRequest<AuthResultDto>;
