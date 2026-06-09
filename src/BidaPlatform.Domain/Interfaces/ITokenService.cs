using System.Security.Claims;
using BidaPlatform.Domain.Entities;

namespace BidaPlatform.Domain.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();

    ClaimsPrincipal? ValidateToken(string token, bool validateLifetime = true);

    int GetAccessTokenExpirationMinutes();
    int GetRefreshTokenExpirationDays();
}
