using MediatR;
using BidaPlatform.Application.Interfaces;
using BidaPlatform.Application.Models.AI;
using BidaPlatform.Domain.Entities;
using BidaPlatform.Domain.Interfaces;

namespace BidaPlatform.Application.UseCases.AI.GetVenueInsights;

public record GetVenueInsightsQuery(Guid ActorUserId, string ActorRole, Guid? ActorVenueId, Guid VenueId, DateTime? FromDate, DateTime? ToDate) : IRequest<AiInsightResponse>;

public class GetVenueInsightsHandler : IRequestHandler<GetVenueInsightsQuery, AiInsightResponse>
{
    private readonly IVenueAccessChecker _venueAccessChecker;
    private readonly ISubscriptionAccessService _subscriptionAccess;
    private readonly IAiInsightService _aiInsightService;
    private readonly IAiAnalysisHistoryRepository _historyRepo;

    public GetVenueInsightsHandler(
        IVenueAccessChecker venueAccessChecker,
        ISubscriptionAccessService subscriptionAccess,
        IAiInsightService aiInsightService,
        IAiAnalysisHistoryRepository historyRepo)
    {
        _venueAccessChecker = venueAccessChecker;
        _subscriptionAccess = subscriptionAccess;
        _aiInsightService = aiInsightService;
        _historyRepo = historyRepo;
    }

    public async Task<AiInsightResponse> Handle(GetVenueInsightsQuery request, CancellationToken ct)
    {
        if (request.ActorRole is not ("SuperAdmin" or "Manager"))
            throw new UnauthorizedAccessException("Chỉ SuperAdmin hoặc Manager mới được xem AI insights");

        await _venueAccessChecker.EnsureCanAccessVenueAsync(request.ActorUserId, request.ActorRole, request.ActorVenueId, request.VenueId, ct);

        await _subscriptionAccess.EnsurePremiumVenueAsync(request.VenueId, ct);

        var result = await _aiInsightService.GetVenueInsightsAsync(request.VenueId, request.FromDate, request.ToDate, ct);

        await _historyRepo.AddAsync(new AiAnalysisHistory
        {
            Id = Guid.NewGuid(),
            VenueId = request.VenueId,
            AnalyzedByUserId = request.ActorUserId,
            Summary = result.Summary,
            Trend = result.Trend,
            Recommendation = result.Recommendation,
            EstimatedNextPeriodRevenue = result.EstimatedNextPeriodRevenue,
            AnalyzedAt = DateTime.UtcNow
        }, ct);
        await _historyRepo.SaveChangesAsync(ct);

        return result;
    }
}
