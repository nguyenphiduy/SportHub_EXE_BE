using BidaPlatform.Domain.Entities;

namespace BidaPlatform.Domain.Interfaces;

public interface IAiAnalysisHistoryRepository
{
    Task<AiAnalysisHistory?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<AiAnalysisHistory>> GetByVenueIdAsync(Guid venueId, DateTime? fromDate, DateTime? toDate, int limit = 20, CancellationToken ct = default);
    Task AddAsync(AiAnalysisHistory history, CancellationToken ct = default);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
