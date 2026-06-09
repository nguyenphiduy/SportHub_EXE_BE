namespace BidaPlatform.Application.Models.AI;

public class AiAnalysisHistoryResponse
{
    public Guid Id { get; set; }
    public Guid VenueId { get; set; }
    public Guid AnalyzedByUserId { get; set; }
    public string Summary { get; set; } = string.Empty;
    public string Trend { get; set; } = string.Empty;
    public string Recommendation { get; set; } = string.Empty;
    public decimal EstimatedNextPeriodRevenue { get; set; }
    public DateTime AnalyzedAt { get; set; }
}
