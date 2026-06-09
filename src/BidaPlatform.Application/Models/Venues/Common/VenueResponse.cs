namespace BidaPlatform.Application.Models.Venues.Common;

public class VenueResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? OwnerName { get; set; }
    public string Status { get; set; } = null!;
    public bool IsActive { get; set; }
    public Guid? PrimaryManagerId { get; set; }
    public string? PrimaryManagerName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public VenueSubscriptionInfo? Subscription { get; set; }
}

public class VenueSubscriptionInfo
{
    public string Plan { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime EndDate { get; set; }
}
