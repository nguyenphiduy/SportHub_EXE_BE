namespace BidaPlatform.Application.Models.Dashboard;

public class DashboardSummaryResponse
{
    public string Scope { get; set; } = null!;
    public Guid? VenueId { get; set; }
    public int TotalVenues { get; set; }
    public int TotalUsers { get; set; }
    public int ActiveTables { get; set; }
    public int ActiveShifts { get; set; }
    public decimal TotalRevenue { get; set; }
}
