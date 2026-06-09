using BidaPlatform.Application.Interfaces;
using BidaPlatform.Application.Models.Dashboard;
using BidaPlatform.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;

namespace BidaPlatform.Infrastructure.Services;

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _dbContext;

    public DashboardService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DashboardSummaryResponse> GetSystemDashboardAsync(CancellationToken ct = default)
    {
        return new DashboardSummaryResponse
        {
            Scope = "System",
            VenueId = null,
            TotalVenues = await _dbContext.Venues.CountAsync(ct),
            TotalUsers = await _dbContext.Users.CountAsync(ct),
            ActiveTables = await _dbContext.BilliardTables.CountAsync(x => x.IsActive, ct),
            ActiveShifts = await _dbContext.WorkShifts.CountAsync(x => x.IsActive, ct),
            TotalRevenue = await _dbContext.BilliardSessions.SumAsync(x => x.TotalPrice ?? 0m, ct)
        };
    }

    public async Task<DashboardSummaryResponse> GetVenueDashboardAsync(Guid venueId, CancellationToken ct = default)
    {
        return new DashboardSummaryResponse
        {
            Scope = "Venue",
            VenueId = venueId,
            TotalVenues = 1,
            TotalUsers = await _dbContext.Users.CountAsync(x => x.VenueId == venueId, ct),
            ActiveTables = await _dbContext.BilliardTables.CountAsync(x => x.VenueId == venueId && x.IsActive, ct),
            ActiveShifts = await _dbContext.WorkShifts.CountAsync(x => x.VenueId == venueId && x.IsActive, ct),
            TotalRevenue = await _dbContext.BilliardSessions.Where(x => x.VenueId == venueId).SumAsync(x => x.TotalPrice ?? 0m, ct)
        };
    }
}
