namespace BidaPlatform.Application.Models.Revenue;

public class RevenueSummaryResponse
{
    public string Period { get; set; } = null!;          // "day" | "week" | "month"
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TableRevenue { get; set; }
    public List<RevenueBreakdownItem> Breakdown { get; set; } = new();
    public List<PeakHourItem> PeakHours { get; set; } = new();
}

public class RevenueBreakdownItem
{
    /// <summary>Display label: "19:00" for day, "T2 06/03" for week, "06/03" for month</summary>
    public string Label { get; set; } = null!;
    public decimal TableRevenue { get; set; }
    public decimal Total { get; set; }
}

public class PeakHourItem
{
    public int Hour { get; set; }
    public string Label { get; set; } = null!;    // "19:00"
    public int SessionCount { get; set; }
    public decimal Revenue { get; set; }
}
