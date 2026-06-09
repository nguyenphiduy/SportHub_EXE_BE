using BidaPlatform.Application.Models.Revenue;

namespace BidaPlatform.Application.Interfaces;

public interface IRevenueService
{
    Task<RevenueSummaryResponse> GetSummaryAsync(
        Guid? venueId,
        string period,
        DateTime anchorDate,
        CancellationToken ct = default);
}
