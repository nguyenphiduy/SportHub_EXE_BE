using BidaPlatform.Application.Models.AI;

namespace BidaPlatform.Application.Interfaces;

public interface IAiInsightService
{
    Task<AiInsightResponse> GetVenueInsightsAsync(Guid venueId, DateTime? fromDate, DateTime? toDate, CancellationToken ct = default);
}
