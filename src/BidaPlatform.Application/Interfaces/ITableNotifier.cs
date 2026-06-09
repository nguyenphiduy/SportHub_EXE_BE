namespace BidaPlatform.Application.Interfaces;

public interface ITableNotifier
{
    Task NotifyTableStatusChangedAsync(Guid venueId, Guid tableId, string tableName, string status, CancellationToken ct = default);
}
