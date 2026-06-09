using MediatR;
using BidaPlatform.Application.Models.Subscriptions.AssignPlan;
using BidaPlatform.Application.Models.Subscriptions.Common;
using BidaPlatform.Domain.Entities;
using BidaPlatform.Domain.Enums;
using BidaPlatform.Domain.Interfaces;

namespace BidaPlatform.Application.UseCases.Subscriptions.AssignSubscription;

public record AssignSubscriptionCommand(Guid SuperAdminId, Guid VenueId, AssignSubscriptionRequest Request) : IRequest<SubscriptionResponse>;

public class AssignSubscriptionHandler : IRequestHandler<AssignSubscriptionCommand, SubscriptionResponse>
{
    private readonly IVenueRepository _venueRepository;
    private readonly IVenueSubscriptionRepository _subscriptionRepository;

    public AssignSubscriptionHandler(IVenueRepository venueRepository, IVenueSubscriptionRepository subscriptionRepository)
    {
        _venueRepository = venueRepository;
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<SubscriptionResponse> Handle(AssignSubscriptionCommand request, CancellationToken ct)
    {
        var venue = await _venueRepository.GetByIdAsync(request.VenueId, ct)
            ?? throw new KeyNotFoundException("Không tìm thấy quán");

        // Deactivate all existing active subscriptions for this venue
        var existingActive = await _subscriptionRepository.GetActiveByVenueIdAsync(request.VenueId, ct);
        foreach (var existing in existingActive)
        {
            existing.Status = VenueSubscriptionStatus.Expired;
            existing.UpdatedAt = DateTime.UtcNow;
            _subscriptionRepository.Update(existing);
        }

        var subscription = new VenueSubscription
        {
            Id = Guid.NewGuid(),
            VenueId = venue.Id,
            Plan = request.Request.Plan,
            Status = VenueSubscriptionStatus.Active,
            StartDate = DateTime.SpecifyKind(request.Request.StartDate, DateTimeKind.Utc),
            EndDate = DateTime.SpecifyKind(request.Request.EndDate, DateTimeKind.Utc),
            AutoRenew = request.Request.AutoRenew,
            ApprovedBySuperAdminId = request.SuperAdminId,
            ApprovedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _subscriptionRepository.AddAsync(subscription, ct);
        await _subscriptionRepository.SaveChangesAsync(ct);

        return Map(subscription);
    }

    private static SubscriptionResponse Map(VenueSubscription subscription) => new()
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
