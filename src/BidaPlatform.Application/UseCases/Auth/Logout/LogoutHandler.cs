using MediatR;
using BidaPlatform.Domain.Interfaces;

namespace BidaPlatform.Application.UseCases.Auth.Logout;

public class LogoutHandler : IRequestHandler<LogoutCommand>
{
    private readonly IAuthTokenRepository _authTokenRepository;

    public LogoutHandler(IAuthTokenRepository authTokenRepository)
    {
        _authTokenRepository = authTokenRepository;
    }

    public async Task Handle(
        LogoutCommand request,
        CancellationToken cancellationToken)
    {
        var token = await _authTokenRepository
            .GetRefreshTokenAsync(request.RefreshToken, cancellationToken);

        if (token == null) return;

        token.IsRevoked = true;
        _authTokenRepository.Update(token);

        await _authTokenRepository.SaveChangeAsync(cancellationToken);
    }
}
