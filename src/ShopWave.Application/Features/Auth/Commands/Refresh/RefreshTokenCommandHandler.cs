using MediatR;
using Microsoft.EntityFrameworkCore;
using ShopWave.Application.Common.Interfaces;
using ShopWave.Application.Features.Auth.DTOs;

namespace ShopWave.Application.Features.Auth.Commands.Refresh;

public class RefreshTokenCommandHandler(
    IAppDbContext db,
    IJwtTokenService jwtTokenService) : IRequestHandler<RefreshTokenCommand, AuthResultDto>
{
    public async Task<AuthResultDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var userId = jwtTokenService.GetUserIdFromExpiredToken(request.AccessToken)
            ?? throw new UnauthorizedAccessException("Invalid access token.");

        var user = await db.Users
            .IgnoreQueryFilters()
            .Include(u => u.Tenant)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
            ?? throw new UnauthorizedAccessException("User not found.");

        if (user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiry < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");

        var newRefreshToken = jwtTokenService.GenerateRefreshToken();
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(30);
        await db.SaveChangesAsync(cancellationToken);

        var accessToken = jwtTokenService.GenerateAccessToken(user);
        return new AuthResultDto(
            accessToken,
            newRefreshToken,
            DateTime.UtcNow.AddMinutes(60),
            user.Email,
            user.Role.ToString(),
            user.TenantId);
    }
}
