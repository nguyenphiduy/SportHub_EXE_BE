using BidaPlatform.Application.Models.Venues.Common;

namespace BidaPlatform.Application.Models.Venues.Detail;

public class VenueDetailResponse : VenueResponse
{
    public string? CurrentPlan { get; set; }
    public string? SubscriptionStatus { get; set; }
    public DateTime? SubscriptionStartDate { get; set; }
    public DateTime? SubscriptionEndDate { get; set; }
}
