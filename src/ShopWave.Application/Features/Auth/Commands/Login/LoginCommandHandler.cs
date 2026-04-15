using MediatR;
using Microsoft.EntityFrameworkCore;
using ShopWave.Application.Common.Interfaces;
using ShopWave.Application.Features.Auth.DTOs;

namespace ShopWave.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler(
    IAppDbContext db,
    IJwtTokenService jwtTokenService) : IRequestHandler<LoginCommand, AuthResultDto>
{
    public async Task<AuthResultDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Use IgnoreQueryFilters to allow login without TenantId in context
        var user = await db.Users
            .IgnoreQueryFilters()
            .Include(u => u.Tenant)
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken)
            ?? throw new UnauthorizedAccessException("Invalid email or password.");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        if (!user.Tenant.IsActive)
            throw new UnauthorizedAccessException("Your account has been deactivated.");

        var refreshToken = jwtTokenService.GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(30);
        await db.SaveChangesAsync(cancellationToken);

        var accessToken = jwtTokenService.GenerateAccessToken(user);
        return new AuthResultDto(
            accessToken,
            refreshToken,
            DateTime.UtcNow.AddMinutes(60),
            user.Email,
            user.Role.ToString(),
            user.TenantId);
    }
}
