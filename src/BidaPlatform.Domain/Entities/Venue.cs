using BidaPlatform.Domain.Enums;

namespace BidaPlatform.Domain.Entities;

public class Venue
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? OwnerName { get; set; }
    public VenueStatus Status { get; set; } = VenueStatus.Pending;
    public bool IsActive { get; set; } = true;
    public Guid? PrimaryManagerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public User? PrimaryManager { get; set; }
    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<BilliardTable> Tables { get; set; } = new List<BilliardTable>();
    public ICollection<BilliardSession> Sessions { get; set; } = new List<BilliardSession>();
    public ICollection<IoTDevice> IoTDevices { get; set; } = new List<IoTDevice>();
    public ICollection<VenueSubscription> Subscriptions { get; set; } = new List<VenueSubscription>();
}
