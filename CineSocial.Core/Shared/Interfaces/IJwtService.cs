using CineSocial.Domain.Entities;

namespace CineSocial.Core.Shared.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
    string GenerateRefreshToken();
    bool ValidateToken(string token);
    Guid? GetUserIdFromToken(string token);
    string? GetClaimFromToken(string token, string claimType);
    int GetTokenExpiryMinutes();
}