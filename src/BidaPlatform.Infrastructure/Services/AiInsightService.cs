using BidaPlatform.Application.Interfaces;
using BidaPlatform.Application.Models.AI;
using BidaPlatform.Domain.Entities;
using BidaPlatform.Domain.Enums;
using BidaPlatform.Domain.Interfaces;
using BidaPlatform.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;

namespace BidaPlatform.Infrastructure.Services;

public class AiInsightService : IAiInsightService
{
    private readonly AppDbContext _dbContext;
    private readonly ISubscriptionAccessService _subscriptionAccessService;
    private readonly IVenueRepository _venueRepository;
    private readonly OpenRouterAiClient _openRouterClient;

    public AiInsightService(
        AppDbContext dbContext,
        ISubscriptionAccessService subscriptionAccessService,
        IVenueRepository venueRepository,
        OpenRouterAiClient openRouterClient)
    {
        _dbContext = dbContext;
        _subscriptionAccessService = subscriptionAccessService;
        _venueRepository = venueRepository;
        _openRouterClient = openRouterClient;
    }

    public async Task<AiInsightResponse> GetVenueInsightsAsync(Guid venueId, DateTime? fromDate, DateTime? toDate, CancellationToken ct = default)
    {
        await _subscriptionAccessService.EnsurePremiumVenueAsync(venueId, ct);

        var venue = await _venueRepository.GetByIdAsync(venueId, ct)
            ?? throw new KeyNotFoundException("Không tìm thấy quán");

        var query = _dbContext.BilliardSessions
            .Where(x => x.VenueId == venueId && x.TotalPrice != null);

        if (fromDate.HasValue)
            query = query.Where(x => x.StartTime >= DateTime.SpecifyKind(fromDate.Value, DateTimeKind.Utc));

        if (toDate.HasValue)
            query = query.Where(x => x.StartTime <= DateTime.SpecifyKind(toDate.Value, DateTimeKind.Utc));

        var completedSessions = await query
            .OrderByDescending(x => x.StartTime)
            .Take(100)
            .ToListAsync(ct);

        var totalRevenue = completedSessions.Sum(x => x.TotalPrice ?? 0m);
        var avgRevenue = completedSessions.Count == 0 ? 0m : totalRevenue / completedSessions.Count;

        var activeTables = await _dbContext.BilliardTables
            .CountAsync(t => t.VenueId == venueId && t.Status == BilliardTableStatus.Available, ct);
        var totalTables = await _dbContext.BilliardTables
            .CountAsync(t => t.VenueId == venueId, ct);

        var prompt = VenueInsightPromptBuilder.BuildPrompt(
            venue, completedSessions, totalRevenue, avgRevenue, activeTables, totalTables);

        var aiResponse = await _openRouterClient.GenerateInsightAsync(prompt, ct);

        if (aiResponse != null)
        {
            aiResponse.VenueId = venueId;
            return aiResponse;
        }

        return BuildHeuristicResponse(venueId, totalRevenue, avgRevenue, completedSessions.Count);
    }

    private static AiInsightResponse BuildHeuristicResponse(
        Guid venueId, decimal totalRevenue, decimal avgRevenue, int sessionCount)
    {
        return new AiInsightResponse
        {
            VenueId = venueId,
            Summary = $"{sessionCount} phiên gần nhất tạo ra tổng doanh thu {totalRevenue:0.##} VND.",
            Trend = sessionCount >= 10
                ? "Lưu lượng chơi ổn định trong ngắn hạn."
                : "Cần thêm dữ liệu để phân tích xu hướng rõ hơn.",
            Recommendation = avgRevenue >= 150000m
                ? "Duy trì cấu hình bàn premium và tối ưu cao điểm."
                : "Tăng upsell khung giờ cao điểm và rà soát giá bàn/ca để cải thiện doanh thu.",
            EstimatedNextPeriodRevenue = Math.Round(avgRevenue * 10, 2)
        };
    }
}
