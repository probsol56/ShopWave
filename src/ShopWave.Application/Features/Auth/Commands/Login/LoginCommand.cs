using MediatR;
using ShopWave.Application.Features.Auth.DTOs;

namespace ShopWave.Application.Features.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<AuthResultDto>;
