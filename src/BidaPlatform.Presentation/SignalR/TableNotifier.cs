using Microsoft.AspNetCore.SignalR;
using BidaPlatform.Application.Interfaces;
using BidaPlatform.Presentation.Hubs;

namespace BidaPlatform.Presentation.SignalR;

public class TableNotifier : ITableNotifier
{
    private readonly IHubContext<TableHub> _hubContext;

    public TableNotifier(IHubContext<TableHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyTableStatusChangedAsync(Guid venueId, Guid tableId, string tableName, string status, CancellationToken ct = default)
    {
        await _hubContext.Clients.Group($"venue:{venueId}").SendAsync("TableStatusChanged", new
        {
            VenueId = venueId,
            TableId = tableId,
            TableName = tableName,
            Status = status
        }, ct);
    }
}
