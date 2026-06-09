using BidaPlatform.Application.Models.Dashboard;

namespace BidaPlatform.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardSummaryResponse> GetSystemDashboardAsync(CancellationToken ct = default);
    Task<DashboardSummaryResponse> GetVenueDashboardAsync(Guid venueId, CancellationToken ct = default);
}
