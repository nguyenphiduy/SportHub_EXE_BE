using BidaPlatform.Domain.Entities;
using BidaPlatform.Domain.Interfaces;
using BidaPlatform.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;

namespace BidaPlatform.Infrastructure.Repositories;

public class AiAnalysisHistoryRepository : IAiAnalysisHistoryRepository
{
    private readonly AppDbContext _db;

    public AiAnalysisHistoryRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<AiAnalysisHistory?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.AiAnalysisHistory.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<IReadOnlyList<AiAnalysisHistory>> GetByVenueIdAsync(Guid venueId, DateTime? fromDate, DateTime? toDate, int limit = 20, CancellationToken ct = default)
    {
        var query = _db.AiAnalysisHistory.Where(x => x.VenueId == venueId);

        if (fromDate.HasValue)
            query = query.Where(x => x.AnalyzedAt >= DateTime.SpecifyKind(fromDate.Value, DateTimeKind.Utc));

        if (toDate.HasValue)
            query = query.Where(x => x.AnalyzedAt <= DateTime.SpecifyKind(toDate.Value, DateTimeKind.Utc));

        return await query
            .OrderByDescending(x => x.AnalyzedAt)
            .Take(limit)
            .ToListAsync(ct);
    }

    public async Task AddAsync(AiAnalysisHistory history, CancellationToken ct = default)
        => await _db.AiAnalysisHistory.AddAsync(history, ct);

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}
