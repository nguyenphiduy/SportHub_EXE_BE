using BidaPlatform.Application.Interfaces;
using BidaPlatform.Domain.Enums;
using BidaPlatform.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;

namespace BidaPlatform.Infrastructure.Services;

public class SubscriptionAccessService : ISubscriptionAccessService
{
    private readonly AppDbContext _dbContext;

    public SubscriptionAccessService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task EnsurePremiumVenueAsync(Guid venueId, CancellationToken ct = default)
    {
        var hasPremium = await HasActivePlanAsync(venueId, SubscriptionPlan.Premium, ct);
        if (!hasPremium)
            throw new UnauthorizedAccessException("Tính năng này chỉ khả dụng với gói Premium còn hiệu lực");
    }

    public Task<bool> HasActivePlanAsync(Guid venueId, SubscriptionPlan plan, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        return _dbContext.VenueSubscriptions.AnyAsync(x =>
            x.VenueId == venueId &&
            x.Plan == plan &&
            x.Status == VenueSubscriptionStatus.Active &&
            x.StartDate <= now &&
            x.EndDate >= now,
            ct);
    }
}
