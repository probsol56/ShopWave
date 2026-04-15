using MediatR;
using Microsoft.EntityFrameworkCore;
using ShopWave.Application.Common.Interfaces;
using ShopWave.Application.Features.Auth.DTOs;
using ShopWave.Domain.Enums;
using TenantEntity = ShopWave.Domain.Entities.Tenant;
using UserEntity = ShopWave.Domain.Entities.User;

namespace ShopWave.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler(
    IAppDbContext db,
    IJwtTokenService jwtTokenService) : IRequestHandler<RegisterCommand, AuthResultDto>
{
    public async Task<AuthResultDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var slugExists = await db.Tenants.AnyAsync(t => t.Slug == request.TenantSlug, cancellationToken);
        if (slugExists)
            throw new InvalidOperationException($"Tenant slug '{request.TenantSlug}' is already taken.");

        var emailExists = await db.Users.AnyAsync(u => u.Email == request.Email, cancellationToken);
        if (emailExists)
            throw new InvalidOperationException($"Email '{request.Email}' is already registered.");

        var tenant = new TenantEntity
        {
            Name = request.TenantName,
            Slug = request.TenantSlug,
            IsActive = true
        };
        db.Tenants.Add(tenant);

        var refreshToken = jwtTokenService.GenerateRefreshToken();
        var user = new UserEntity
        {
            TenantId = tenant.Id,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Role = UserRole.Owner,
            RefreshToken = refreshToken,
            RefreshTokenExpiry = DateTime.UtcNow.AddDays(30)
        };
        db.Users.Add(user);

        await db.SaveChangesAsync(cancellationToken);

        var accessToken = jwtTokenService.GenerateAccessToken(user);
        return new AuthResultDto(
            accessToken,
            refreshToken,
            DateTime.UtcNow.AddMinutes(60),
            user.Email,
            user.Role.ToString(),
            tenant.Id);
    }
}
