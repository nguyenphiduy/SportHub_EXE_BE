using Microsoft.AspNetCore.SignalR;

namespace BidaPlatform.Presentation.Hubs;

public class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var venueId = Context.GetHttpContext()?.Request.Query["venueId"].ToString();
        if (!string.IsNullOrWhiteSpace(venueId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"venue:{venueId}");
        }

        await base.OnConnectedAsync();
    }
}
