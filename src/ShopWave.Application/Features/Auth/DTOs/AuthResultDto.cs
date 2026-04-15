namespace ShopWave.Application.Features.Auth.DTOs;

public record AuthResultDto(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    string Email,
    string Role,
    Guid TenantId);
