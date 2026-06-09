using BidaPlatform.Domain.Enums;

namespace BidaPlatform.Application.Interfaces;

public interface ISubscriptionAccessService
{
    Task EnsurePremiumVenueAsync(Guid venueId, CancellationToken ct = default);
    Task<bool> HasActivePlanAsync(Guid venueId, SubscriptionPlan plan, CancellationToken ct = default);
}
