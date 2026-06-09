using MediatR;

namespace BidaPlatform.Application.UseCases.Users.CreateUser;

public class CreateUserCommand : IRequest
{
    public Guid CreatorUserId { get; }
    public string Email { get; }
    public string FullName { get; }
    public Guid VenueId { get; }

    public CreateUserCommand(
        Guid creatorUserId,
        string email,
        string fullName,
        Guid venueId)
    {
        CreatorUserId = creatorUserId;
        Email = email;
        FullName = fullName;
        VenueId = venueId;
    }
}
