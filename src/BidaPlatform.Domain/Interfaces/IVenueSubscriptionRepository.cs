using BidaPlatform.Domain.Entities;

namespace BidaPlatform.Domain.Interfaces;

public interface IVenueSubscriptionRepository
{
    Task AddAsync(VenueSubscription subscription, CancellationToken ct = default);
    Task<VenueSubscription?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<VenueSubscription?> GetCurrentByVenueIdAsync(Guid venueId, CancellationToken ct = default);
    Task<List<VenueSubscription>> GetByVenueIdAsync(Guid venueId, CancellationToken ct = default);
    Task<List<VenueSubscription>> GetActiveExpiredAsync(DateTime now, CancellationToken ct = default);
    Task<List<VenueSubscription>> GetActiveByVenueIdAsync(Guid venueId, CancellationToken ct = default);
    void Update(VenueSubscription subscription);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
