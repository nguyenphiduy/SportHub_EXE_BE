using MediatR;
using BidaPlatform.Application.Models.Auth.Login;

namespace BidaPlatform.Application.UseCases.Auth.Login;

public class LoginCommand : IRequest<LoginResponse>
{
    public string Email { get; }
    public string Password { get; }

    public LoginCommand(string email, string password)
    {
        Email = email;
        Password = password;
    }
}
