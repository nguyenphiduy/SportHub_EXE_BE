using MediatR;
using BidaPlatform.Application.Interfaces;
using BidaPlatform.Application.Models.AI;
using BidaPlatform.Domain.Interfaces;

namespace BidaPlatform.Application.UseCases.AI.GetAiAnalysisHistory;

public record GetAiAnalysisHistoryQuery(
    Guid ActorUserId,
    string ActorRole,
    Guid? ActorVenueId,
    Guid VenueId,
    DateTime? FromDate,
    DateTime? ToDate) : IRequest<IReadOnlyList<AiAnalysisHistoryResponse>>;

public class GetAiAnalysisHistoryHandler : IRequestHandler<GetAiAnalysisHistoryQuery, IReadOnlyList<AiAnalysisHistoryResponse>>
{
    private readonly IVenueAccessChecker _venueAccessChecker;
    private readonly IAiAnalysisHistoryRepository _historyRepo;

    public GetAiAnalysisHistoryHandler(
        IVenueAccessChecker venueAccessChecker,
        IAiAnalysisHistoryRepository historyRepo)
    {
        _venueAccessChecker = venueAccessChecker;
        _historyRepo = historyRepo;
    }

    public async Task<IReadOnlyList<AiAnalysisHistoryResponse>> Handle(GetAiAnalysisHistoryQuery request, CancellationToken ct)
    {
        await _venueAccessChecker.EnsureCanAccessVenueAsync(
            request.ActorUserId, request.ActorRole, request.ActorVenueId, request.VenueId, ct);

        var history = await _historyRepo.GetByVenueIdAsync(request.VenueId, request.FromDate, request.ToDate, 20, ct);

        return history.Select(h => new AiAnalysisHistoryResponse
        {
            Id = h.Id,
            VenueId = h.VenueId,
            AnalyzedByUserId = h.AnalyzedByUserId,
            Summary = h.Summary,
            Trend = h.Trend,
            Recommendation = h.Recommendation,
            EstimatedNextPeriodRevenue = h.EstimatedNextPeriodRevenue,
            AnalyzedAt = h.AnalyzedAt
        }).ToList();
    }
}
