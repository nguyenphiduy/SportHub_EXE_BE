using MediatR;

namespace BidaPlatform.Application.UseCases.Auth.ChangePassword;

public class ChangePasswordCommand : IRequest
{
    public Guid UserId { get; }
    public string CurrentPassword { get; }
    public string NewPassword { get; }

    public ChangePasswordCommand(
        Guid userId,
        string currentPassword,
        string newPassword)
    {
        UserId = userId;
        CurrentPassword = currentPassword;
        NewPassword = newPassword;
    }
}
