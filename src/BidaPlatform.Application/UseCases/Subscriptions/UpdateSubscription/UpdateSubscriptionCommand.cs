using MediatR;
using BidaPlatform.Application.Models.Subscriptions.Common;
using BidaPlatform.Application.Models.Subscriptions.UpdatePlan;
using BidaPlatform.Domain.Interfaces;

namespace BidaPlatform.Application.UseCases.Subscriptions.UpdateSubscription;

public record UpdateSubscriptionCommand(Guid SubscriptionId, UpdateSubscriptionRequest Request) : IRequest<SubscriptionResponse>;

public class UpdateSubscriptionHandler : IRequestHandler<UpdateSubscriptionCommand, SubscriptionResponse>
{
    private readonly IVenueSubscriptionRepository _subscriptionRepository;

    public UpdateSubscriptionHandler(IVenueSubscriptionRepository subscriptionRepository)
    {
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<SubscriptionResponse> Handle(UpdateSubscriptionCommand request, CancellationToken ct)
    {
        var subscription = await _subscriptionRepository.GetByIdAsync(request.SubscriptionId, ct)
            ?? throw new KeyNotFoundException("Không tìm thấy gói quán");

        if (request.Request.Plan.HasValue) subscription.Plan = request.Request.Plan.Value;
        if (request.Request.StartDate.HasValue) subscription.StartDate = DateTime.SpecifyKind(request.Request.StartDate.Value, DateTimeKind.Utc);
        if (request.Request.EndDate.HasValue) subscription.EndDate = DateTime.SpecifyKind(request.Request.EndDate.Value, DateTimeKind.Utc);
        if (request.Request.Status.HasValue) subscription.Status = request.Request.Status.Value;
        if (request.Request.AutoRenew.HasValue) subscription.AutoRenew = request.Request.AutoRenew.Value;
        subscription.UpdatedAt = DateTime.UtcNow;

        _subscriptionRepository.Update(subscription);
        await _subscriptionRepository.SaveChangesAsync(ct);

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
