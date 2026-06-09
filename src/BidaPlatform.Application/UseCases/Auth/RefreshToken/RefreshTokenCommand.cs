using MediatR;
using BidaPlatform.Application.Models.Auth.RefreshToken;

namespace BidaPlatform.Application.UseCases.Auth.RefreshToken;

public class RefreshTokenCommand : IRequest<RefreshTokenResponse>
{
    public string RefreshToken { get; }

    public RefreshTokenCommand(string refreshToken)
    {
        RefreshToken = refreshToken;
    }
}
