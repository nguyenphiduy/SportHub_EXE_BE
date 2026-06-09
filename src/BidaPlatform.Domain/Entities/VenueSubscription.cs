using BidaPlatform.Domain.Enums;

namespace BidaPlatform.Domain.Entities;

public class VenueSubscription
{
    public Guid Id { get; set; }
    public Guid VenueId { get; set; }
    public SubscriptionPlan Plan { get; set; }
    public VenueSubscriptionStatus Status { get; set; } = VenueSubscriptionStatus.PendingApproval;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool AutoRenew { get; set; }
    public Guid? ApprovedBySuperAdminId { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Venue Venue { get; set; } = null!;
    public User? ApprovedBySuperAdmin { get; set; }
}
