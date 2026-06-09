namespace BidaPlatform.Domain.Entities;

public class IoTDevice
{
    public Guid Id { get; set; }
    public Guid VenueId { get; set; }
    public Guid TableId { get; set; }
    public string IpAddress { get; set; } = null!;
    public string? DeviceName { get; set; }
    public bool IsOnline { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation
    public Venue Venue { get; set; } = null!;
    public BilliardTable Table { get; set; } = null!;
}
