using BidaPlatform.Application.Interfaces;
using BidaPlatform.Application.Models.Revenue;
using BidaPlatform.Domain.Enums;
using BidaPlatform.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;

namespace BidaPlatform.Infrastructure.Services;

public class RevenueService : IRevenueService
{
    private readonly AppDbContext _db;
    private static readonly TimeSpan VnOffset = TimeSpan.FromHours(7);

    public RevenueService(AppDbContext db) => _db = db;

    private record SessionRevItem(DateTime StartTimeUtc, decimal Revenue);

    public async Task<RevenueSummaryResponse> GetSummaryAsync(
        Guid? venueId,
        string period,
        DateTime anchorDate,
        CancellationToken ct = default)
    {
        DateTime vnStart, vnEnd;

        switch (period)
        {
            case "day":
                vnStart = anchorDate.Date;
                vnEnd = vnStart.AddDays(1);
                break;
            case "week":
                var dow = (int)anchorDate.DayOfWeek;
                var daysToMon = dow == 0 ? 6 : dow - 1;
                vnStart = anchorDate.Date.AddDays(-daysToMon);
                vnEnd = vnStart.AddDays(7);
                break;
            case "year":
                vnStart = new DateTime(anchorDate.Year, 1, 1);
                vnEnd = vnStart.AddYears(1);
                break;
            default:
                vnStart = new DateTime(anchorDate.Year, anchorDate.Month, 1);
                vnEnd = vnStart.AddMonths(1);
                break;
        }

        DateTime utcStart = DateTime.SpecifyKind(vnStart - VnOffset, DateTimeKind.Utc);
        DateTime utcEnd = DateTime.SpecifyKind(vnEnd - VnOffset, DateTimeKind.Utc);

        var query = _db.BilliardSessions
            .Where(s => s.Status == BilliardSessionStatus.Completed
                        && s.PaymentStatus == BilliardPaymentStatus.Paid
                        && s.TotalPrice != null
                        && s.StartTime >= utcStart
                        && s.StartTime < utcEnd);

        if (venueId.HasValue)
        {
            query = query.Where(s => s.VenueId == venueId.Value);
        }

        var sessions = (await query
            .Select(s => new { s.StartTime, TotalPrice = s.TotalPrice!.Value })
            .ToListAsync(ct))
            .Select(s => new SessionRevItem(s.StartTime, s.TotalPrice))
            .ToList();

        decimal tableRevenue = sessions.Sum(s => s.Revenue);

        var breakdown = period switch
        {
            "day" => BuildHourlyBreakdown(sessions),
            "week" => BuildDailyBreakdown(vnStart, 7, sessions),
            "year" => BuildMonthlyBreakdown(vnStart, sessions),
            _ => BuildDailyBreakdown(vnStart, (int)(vnEnd - vnStart).TotalDays, sessions)
        };

        var peakHours = Enumerable.Range(0, 24)
            .Select(h =>
            {
                var inHour = sessions.Where(s => ToVn(s.StartTimeUtc).Hour == h).ToList();
                return new PeakHourItem
                {
                    Hour = h,
                    Label = $"{h:D2}:00",
                    SessionCount = inHour.Count,
                    Revenue = inHour.Sum(s => s.Revenue)
                };
            })
            .Where(p => p.SessionCount > 0)
            .OrderBy(p => p.Hour)
            .ToList();

        return new RevenueSummaryResponse
        {
            Period = period,
            StartDate = vnStart,
            EndDate = vnEnd.AddDays(-1),
            TotalRevenue = tableRevenue,
            TableRevenue = tableRevenue,
            Breakdown = breakdown,
            PeakHours = peakHours
        };
    }

    private static DateTime ToVn(DateTime utc) => utc.Add(VnOffset);

    private static List<RevenueBreakdownItem> BuildHourlyBreakdown(List<SessionRevItem> sessions)
    {
        return Enumerable.Range(0, 24).Select(h =>
        {
            decimal tRev = sessions.Where(s => ToVn(s.StartTimeUtc).Hour == h).Sum(s => s.Revenue);
            return new RevenueBreakdownItem
            {
                Label = $"{h:D2}:00",
                TableRevenue = tRev,
                Total = tRev
            };
        }).ToList();
    }

    private static List<RevenueBreakdownItem> BuildDailyBreakdown(DateTime vnStart, int days, List<SessionRevItem> sessions)
    {
        return Enumerable.Range(0, days).Select(d =>
        {
            var date = vnStart.AddDays(d);
            decimal tRev = sessions.Where(s => ToVn(s.StartTimeUtc).Date == date).Sum(s => s.Revenue);

            var label = days <= 7
                ? date.ToString("ddd dd/MM")
                : date.ToString("dd/MM");

            return new RevenueBreakdownItem
            {
                Label = label,
                TableRevenue = tRev,
                Total = tRev
            };
        }).ToList();
    }

    private static List<RevenueBreakdownItem> BuildMonthlyBreakdown(DateTime vnStart, List<SessionRevItem> sessions)
    {
        return Enumerable.Range(0, 12).Select(m =>
        {
            var monthStart = new DateTime(vnStart.Year, m + 1, 1);
            var monthEnd = monthStart.AddMonths(1);

            decimal tRev = sessions
                .Where(s =>
                {
                    var vn = ToVn(s.StartTimeUtc);
                    return vn >= monthStart && vn < monthEnd;
                })
                .Sum(s => s.Revenue);

            return new RevenueBreakdownItem
            {
                Label = $"Thg {m + 1}",
                TableRevenue = tRev,
                Total = tRev
            };
        }).ToList();
    }
}
