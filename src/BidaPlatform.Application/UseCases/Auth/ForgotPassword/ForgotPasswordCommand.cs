using MediatR;

namespace BidaPlatform.Application.UseCases.Auth.ForgotPassword;

public class ForgotPasswordCommand : IRequest
{
    public string Email { get; }

    public ForgotPasswordCommand(string email)
    {
        Email = email;
    }
}
