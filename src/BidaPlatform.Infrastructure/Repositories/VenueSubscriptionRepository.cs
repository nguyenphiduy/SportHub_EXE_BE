using BidaPlatform.Domain.Entities;
using BidaPlatform.Domain.Interfaces;
using BidaPlatform.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;

namespace BidaPlatform.Infrastructure.Repositories;

public class VenueSubscriptionRepository : IVenueSubscriptionRepository
{
    private readonly AppDbContext _db;

    public VenueSubscriptionRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(VenueSubscription subscription, CancellationToken ct = default)
        => await _db.VenueSubscriptions.AddAsync(subscription, ct);

    public Task<VenueSubscription?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.VenueSubscriptions.FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<VenueSubscription?> GetCurrentByVenueIdAsync(Guid venueId, CancellationToken ct = default)
        => _db.VenueSubscriptions
            .Where(x => x.VenueId == venueId)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync(ct);

    public Task<List<VenueSubscription>> GetByVenueIdAsync(Guid venueId, CancellationToken ct = default)
        => _db.VenueSubscriptions
            .Where(x => x.VenueId == venueId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);

    public Task<List<VenueSubscription>> GetActiveExpiredAsync(DateTime now, CancellationToken ct = default)
        => _db.VenueSubscriptions
            .Where(x => x.Status == BidaPlatform.Domain.Enums.VenueSubscriptionStatus.Active && x.EndDate < now)
            .ToListAsync(ct);

    public Task<List<VenueSubscription>> GetActiveByVenueIdAsync(Guid venueId, CancellationToken ct = default)
        => _db.VenueSubscriptions
            .Where(x => x.VenueId == venueId && x.Status == BidaPlatform.Domain.Enums.VenueSubscriptionStatus.Active)
            .ToListAsync(ct);

    public void Update(VenueSubscription subscription)
        => _db.VenueSubscriptions.Update(subscription);

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}
