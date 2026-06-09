namespace BidaPlatform.Application.Interfaces;

public interface INotificationBroadcaster
{
    Task BroadcastAsync(string entity, string action, Guid? venueId = null);
}
