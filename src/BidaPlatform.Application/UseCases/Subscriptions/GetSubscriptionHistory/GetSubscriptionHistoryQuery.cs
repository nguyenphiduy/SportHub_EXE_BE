using MediatR;
using BidaPlatform.Application.Models.Subscriptions.Common;
using BidaPlatform.Domain.Interfaces;

namespace BidaPlatform.Application.UseCases.Subscriptions.GetSubscriptionHistory;

public record GetSubscriptionHistoryQuery(Guid VenueId) : IRequest<List<SubscriptionResponse>>;

public class GetSubscriptionHistoryHandler : IRequestHandler<GetSubscriptionHistoryQuery, List<SubscriptionResponse>>
{
    private readonly IVenueSubscriptionRepository _subscriptionRepository;

    public GetSubscriptionHistoryHandler(IVenueSubscriptionRepository subscriptionRepository)
    {
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<List<SubscriptionResponse>> Handle(GetSubscriptionHistoryQuery request, CancellationToken ct)
    {
        var subscriptions = await _subscriptionRepository.GetByVenueIdAsync(request.VenueId, ct);
        return subscriptions.Select(subscription => new SubscriptionResponse
        {
            Id = subscription.Id,
            VenueId = subscription.VenueId,
            Plan = subscription.Plan.ToString(),
            Status = subscription.Status.ToString(),
            StartDate = subscription.StartDate,
            EndDate = subscription.EndDate,
            AutoRenew = subscription.AutoRenew,
            ApprovedBySuperAdminId = subscription.ApprovedBySuperAdminId,
            ApprovedAt = subscription.ApprovedAt,
            CreatedAt = subscription.CreatedAt,
            UpdatedAt = subscription.UpdatedAt
        }).ToList();
    }
}
