using BidaPlatform.Domain.Enums;

namespace BidaPlatform.Application.Models.Subscriptions.UpdatePlan;

public class UpdateSubscriptionRequest
{
    public SubscriptionPlan? Plan { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public VenueSubscriptionStatus? Status { get; set; }
    public bool? AutoRenew { get; set; }
}
