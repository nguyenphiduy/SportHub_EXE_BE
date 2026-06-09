namespace BidaPlatform.Application.Interfaces;

public interface IVenueAccessChecker
{
    Task EnsureCanAccessVenueAsync(Guid actorUserId, string actorRole, Guid? actorVenueId, Guid targetVenueId, CancellationToken ct = default);
    Task EnsureCanManageVenueAsync(Guid actorUserId, string actorRole, Guid? actorVenueId, Guid targetVenueId, CancellationToken ct = default);
    Task EnsureVenueApprovedAsync(Guid venueId, CancellationToken ct = default);
}
