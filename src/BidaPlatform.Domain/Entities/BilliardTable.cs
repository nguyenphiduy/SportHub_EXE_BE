using BidaPlatform.Domain.Enums;

namespace BidaPlatform.Domain.Entities;

public class BilliardTable
{
    public Guid Id { get; set; }
    public Guid VenueId { get; set; }
    public string Name { get; set; } = null!;
    public BilliardTableType Type { get; set; }
    public decimal PricePerHour { get; set; }
    public BilliardTableStatus Status { get; set; } = BilliardTableStatus.Available;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation
    public Venue Venue { get; set; } = null!;
    public IoTDevice? IoTDevice { get; set; }
    public ICollection<BilliardSession> Sessions { get; set; } = new List<BilliardSession>();
}
