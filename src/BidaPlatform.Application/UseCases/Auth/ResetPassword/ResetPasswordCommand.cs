using MediatR;

namespace BidaPlatform.Application.UseCases.Auth.ResetPassword;

public class ResetPasswordCommand : IRequest
{
    public string Token { get; }
    public string NewPassword { get; }

    public ResetPasswordCommand(string token, string newPassword)
    {
        Token = token;
        NewPassword = newPassword;
    }
}
