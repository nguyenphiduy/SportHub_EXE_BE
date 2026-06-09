using MediatR;
using BidaPlatform.Application.Models.Venues.Common;
using BidaPlatform.Domain.Interfaces;

namespace BidaPlatform.Application.UseCases.Venues.GetAllVenues;

public record GetAllVenuesQuery() : IRequest<List<VenueResponse>>;

public class GetAllVenuesHandler : IRequestHandler<GetAllVenuesQuery, List<VenueResponse>>
{
    private readonly IVenueRepository _venueRepository;
    private readonly IVenueSubscriptionRepository _subscriptionRepository;

    public GetAllVenuesHandler(IVenueRepository venueRepository, IVenueSubscriptionRepository subscriptionRepository)
    {
        _venueRepository = venueRepository;
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<List<VenueResponse>> Handle(GetAllVenuesQuery request, CancellationToken ct)
    {
        var venues = await _venueRepository.GetAllAsync(ct);
        var result = new List<VenueResponse>();

        foreach (var x in venues)
        {
            var activeSubscription = x.Subscriptions
                .OrderByDescending(s => s.EndDate)
                .FirstOrDefault(s => s.Status == Domain.Enums.VenueSubscriptionStatus.Active);

            result.Add(new VenueResponse
            {
                Id = x.Id,
                Name = x.Name,
                Address = x.Address,
                Phone = x.Phone,
                OwnerName = x.OwnerName,
                Status = x.Status.ToString(),
                IsActive = x.IsActive,
                PrimaryManagerId = x.PrimaryManagerId,
                PrimaryManagerName = x.PrimaryManager?.FullName,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Subscription = activeSubscription != null ? new VenueSubscriptionInfo
                {
                    Plan = activeSubscription.Plan.ToString(),
                    Status = activeSubscription.Status.ToString(),
                    EndDate = activeSubscription.EndDate
                } : null
            });
        }

        return result;
    }
}
