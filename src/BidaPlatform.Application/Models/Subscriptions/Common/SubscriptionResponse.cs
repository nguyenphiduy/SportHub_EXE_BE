namespace BidaPlatform.Application.Models.Subscriptions.Common;

public class SubscriptionResponse
{
    public Guid Id { get; set; }
    public Guid VenueId { get; set; }
    public string Plan { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool AutoRenew { get; set; }
    public Guid? ApprovedBySuperAdminId { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
