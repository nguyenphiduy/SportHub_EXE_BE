using MediatR;
using BidaPlatform.Application.Models.Subscriptions.Common;
using BidaPlatform.Domain.Interfaces;

namespace BidaPlatform.Application.UseCases.Subscriptions.GetCurrentSubscription;

public record GetCurrentSubscriptionQuery(Guid VenueId) : IRequest<SubscriptionResponse>;

public class GetCurrentSubscriptionHandler : IRequestHandler<GetCurrentSubscriptionQuery, SubscriptionResponse>
{
    private readonly IVenueSubscriptionRepository _subscriptionRepository;

    public GetCurrentSubscriptionHandler(IVenueSubscriptionRepository subscriptionRepository)
    {
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<SubscriptionResponse> Handle(GetCurrentSubscriptionQuery request, CancellationToken ct)
    {
        var subscription = await _subscriptionRepository.GetCurrentByVenueIdAsync(request.VenueId, ct)
            ?? throw new KeyNotFoundException("Quán chưa có gói dịch vụ");

        return new SubscriptionResponse
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
        };
    }
}
