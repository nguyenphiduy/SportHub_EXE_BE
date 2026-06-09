namespace BidaPlatform.Application.Interfaces;

public interface ICurrentUserContext
{
    Guid UserId { get; }
    string Role { get; }
    Guid? VenueId { get; }
    bool IsAuthenticated { get; }
}
