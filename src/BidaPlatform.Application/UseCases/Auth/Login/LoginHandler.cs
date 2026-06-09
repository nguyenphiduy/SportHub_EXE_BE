using MediatR;
using BidaPlatform.Domain.Interfaces;
using BidaPlatform.Domain.Entities;
using BidaPlatform.Application.Models.Auth.Login;

namespace BidaPlatform.Application.UseCases.Auth.Login;

public class LoginHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthTokenRepository _authTokenRepository;
    private readonly ITokenService _tokenService;

    public LoginHandler(
        IUserRepository userRepository,
        IAuthTokenRepository authTokenRepository,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _authTokenRepository = authTokenRepository;
        _tokenService = tokenService;
    }

    public async Task<LoginResponse> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user == null || !user.IsActive)
            throw new UnauthorizedAccessException("Email hoặc mật khẩu không đúng");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            throw new UnauthorizedAccessException("Email hoặc mật khẩu không đúng");

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();

        var authToken = new AuthToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiresAt = DateTime.UtcNow
                .AddMinutes(_tokenService.GetAccessTokenExpirationMinutes()),
            RefreshTokenExpiresAt = DateTime.UtcNow
                .AddDays(_tokenService.GetRefreshTokenExpirationDays()),
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow
        };

        await _authTokenRepository.SaveTokenAsync(authToken, cancellationToken);
        await _authTokenRepository.SaveChangeAsync(cancellationToken);

        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiresAt = authToken.AccessTokenExpiresAt
        };
    }
}
