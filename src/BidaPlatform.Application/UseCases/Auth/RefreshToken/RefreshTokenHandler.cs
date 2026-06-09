using MediatR;
using BidaPlatform.Application.Models.Auth.RefreshToken;
using BidaPlatform.Domain.Entities;
using BidaPlatform.Domain.Interfaces;

namespace BidaPlatform.Application.UseCases.Auth.RefreshToken;

public class RefreshTokenHandler
    : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    private readonly IAuthTokenRepository _authTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public RefreshTokenHandler(
        IAuthTokenRepository authTokenRepository,
        IUserRepository userRepository,
        ITokenService tokenService)
    {
        _authTokenRepository = authTokenRepository;
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<RefreshTokenResponse> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var oldToken = await _authTokenRepository
            .GetRefreshTokenAsync(request.RefreshToken, cancellationToken);

        if (oldToken == null ||
            oldToken.IsRevoked ||
            oldToken.RefreshTokenExpiresAt < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Refresh token không hợp lệ");

        var user = await _userRepository
            .GetByIdAsync(oldToken.UserId, cancellationToken);

        if (user == null || !user.IsActive)
            throw new UnauthorizedAccessException("User không hợp lệ");

        // 🔥 REVOKE REFRESH TOKEN CŨ
        oldToken.IsRevoked = true;
        _authTokenRepository.Update(oldToken);

        // 🔥 SINH TOKEN MỚI
        var newAccessToken = _tokenService.GenerateAccessToken(user);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        var newToken = new AuthToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            AccessTokenExpiresAt = DateTime.UtcNow
                .AddMinutes(_tokenService.GetAccessTokenExpirationMinutes()),
            RefreshTokenExpiresAt = DateTime.UtcNow
                .AddDays(_tokenService.GetRefreshTokenExpirationDays()),
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow
        };

        await _authTokenRepository.SaveTokenAsync(newToken, cancellationToken);
        await _authTokenRepository.SaveChangeAsync(cancellationToken);

        return new RefreshTokenResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            AccessTokenExpiresAt = newToken.AccessTokenExpiresAt
        };
    }
}
