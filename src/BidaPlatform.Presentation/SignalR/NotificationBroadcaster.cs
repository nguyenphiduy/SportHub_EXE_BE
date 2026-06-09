using Microsoft.AspNetCore.SignalR;
using BidaPlatform.Application.Interfaces;
using BidaPlatform.Presentation.Hubs;

namespace BidaPlatform.Presentation.SignalR;

public class NotificationBroadcaster : INotificationBroadcaster
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationBroadcaster(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task BroadcastAsync(string entity, string action, Guid? venueId = null)
    {
        var payload = new
        {
            Entity = entity,
            Action = action,
            VenueId = venueId,
            Timestamp = DateTime.UtcNow
        };

        if (venueId.HasValue)
        {
            await _hubContext.Clients.Group($"venue:{venueId}").SendAsync("EntityChanged", payload);
            return;
        }

        await _hubContext.Clients.All.SendAsync("EntityChanged", payload);
    }
}
