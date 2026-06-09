using BidaPlatform.Domain.Enums;

namespace BidaPlatform.Domain.Entities;

public class BilliardSession
{
    public Guid Id { get; set; }
    public Guid VenueId { get; set; }
    public Guid TableId { get; set; }
    public Guid StartedByUserId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int? DurationMinutes { get; set; }
    public decimal? TotalPrice { get; set; }
    public BilliardSessionStatus Status { get; set; } = BilliardSessionStatus.Active;
    public string? Note { get; set; }
    public BilliardPaymentMethod? PaymentMethod { get; set; }
    public BilliardPaymentStatus? PaymentStatus { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation
    public Venue Venue { get; set; } = null!;
    public BilliardTable Table { get; set; } = null!;
    public User StartedByUser { get; set; } = null!;
}
