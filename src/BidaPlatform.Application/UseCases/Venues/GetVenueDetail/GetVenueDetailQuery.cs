using MediatR;
using BidaPlatform.Application.Models.Venues.Detail;
using BidaPlatform.Domain.Interfaces;

namespace BidaPlatform.Application.UseCases.Venues.GetVenueDetail;

public record GetVenueDetailQuery(Guid VenueId) : IRequest<VenueDetailResponse>;

public class GetVenueDetailHandler : IRequestHandler<GetVenueDetailQuery, VenueDetailResponse>
{
    private readonly IVenueRepository _venueRepository;
    private readonly IVenueSubscriptionRepository _subscriptionRepository;

    public GetVenueDetailHandler(IVenueRepository venueRepository, IVenueSubscriptionRepository subscriptionRepository)
    {
        _venueRepository = venueRepository;
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<VenueDetailResponse> Handle(GetVenueDetailQuery request, CancellationToken ct)
    {
        var venue = await _venueRepository.GetByIdWithManagerAsync(request.VenueId, ct)
            ?? throw new KeyNotFoundException("Không tìm thấy quán");

        var subscription = await _subscriptionRepository.GetCurrentByVenueIdAsync(request.VenueId, ct);

        return new VenueDetailResponse
        {
            Id = venue.Id,
            Name = venue.Name,
            Address = venue.Address,
            Phone = venue.Phone,
            OwnerName = venue.OwnerName,
            Status = venue.Status.ToString(),
            IsActive = venue.IsActive,
            PrimaryManagerId = venue.PrimaryManagerId,
            PrimaryManagerName = venue.PrimaryManager?.FullName,
            CreatedAt = venue.CreatedAt,
            UpdatedAt = venue.UpdatedAt,
            CurrentPlan = subscription?.Plan.ToString(),
            SubscriptionStatus = subscription?.Status.ToString(),
            SubscriptionStartDate = subscription?.StartDate,
            SubscriptionEndDate = subscription?.EndDate
        };
    }
}
