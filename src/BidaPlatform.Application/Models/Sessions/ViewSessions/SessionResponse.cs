namespace BidaPlatform.Application.Models.Sessions.ViewSessions;

public class SessionResponse
{
    public Guid Id { get; set; }
    public Guid TableId { get; set; }
    public string TableName { get; set; } = null!;
    public string StartedByUserName { get; set; } = null!;
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int? DurationMinutes { get; set; }
    public decimal? TotalPrice { get; set; }
    public string Status { get; set; } = null!;
    public string? Note { get; set; }
    public string? PaymentMethod { get; set; }
    public string? PaymentStatus { get; set; }
}
