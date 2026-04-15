using MediatR;
using ShopWave.Application.Features.Auth.DTOs;

namespace ShopWave.Application.Features.Auth.Commands.Refresh;

public record RefreshTokenCommand(string AccessToken, string RefreshToken) : IRequest<AuthResultDto>;
