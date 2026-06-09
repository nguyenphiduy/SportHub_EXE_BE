using MediatR;

namespace BidaPlatform.Application.UseCases.Users.UpdateUser;

public class UpdateProfileCommand : IRequest
{
    public Guid UserId { get; }
    public string Role { get; }
    public string? Email { get; }
    public string? FullName { get; }

    public UpdateProfileCommand(
        Guid userId,
        string role,
        string? email,
        string? fullName)
    {
        UserId = userId;
        Role = role;
        Email = email;
        FullName = fullName;
    }
}
