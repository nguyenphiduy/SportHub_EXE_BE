namespace BidaPlatform.Application.Models.AI;

public class AiInsightResponse
{
    public Guid VenueId { get; set; }
    public string Summary { get; set; } = null!;
    public string Trend { get; set; } = null!;
    public string Recommendation { get; set; } = null!;
    public decimal EstimatedNextPeriodRevenue { get; set; }
}
