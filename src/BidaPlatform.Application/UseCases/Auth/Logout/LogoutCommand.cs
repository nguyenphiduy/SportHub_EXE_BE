using MediatR;

namespace BidaPlatform.Application.UseCases.Auth.Logout;

public class LogoutCommand : IRequest
{
    public string RefreshToken { get; }

    public LogoutCommand(string refreshToken)
    {
        RefreshToken = refreshToken;
    }
}
