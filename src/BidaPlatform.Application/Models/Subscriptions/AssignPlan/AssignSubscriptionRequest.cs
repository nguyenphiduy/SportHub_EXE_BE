using BidaPlatform.Domain.Enums;

namespace BidaPlatform.Application.Models.Subscriptions.AssignPlan;

public class AssignSubscriptionRequest
{
    public SubscriptionPlan Plan { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool AutoRenew { get; set; }
}
