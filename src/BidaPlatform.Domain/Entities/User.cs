namespace BidaPlatform.Domain.Entities;

public class User
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Role { get; set; } = null!;
    public Guid? VenueId { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation
    public Venue? Venue { get; set; }
    public ICollection<Venue> ManagedVenues { get; set; } = new List<Venue>();
    public ICollection<VenueSubscription> ApprovedSubscriptions { get; set; } = new List<VenueSubscription>();
    public ICollection<AuthToken> AuthTokens { get; set; } = new List<AuthToken>();
}
