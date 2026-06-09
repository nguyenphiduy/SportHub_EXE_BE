using BidaPlatform.Domain.Entities;
using BidaPlatform.Domain.Enums;

namespace BidaPlatform.Domain.Interfaces;

public interface IBilliardTableRepository
{
    Task<IEnumerable<BilliardTable>> GetAllAsync(CancellationToken ct = default);
    Task<IEnumerable<BilliardTable>> GetActiveAsync(CancellationToken ct = default);
    Task<IEnumerable<BilliardTable>> GetAllByVenueAsync(Guid venueId, CancellationToken ct = default);
    Task<IEnumerable<BilliardTable>> GetActiveByVenueAsync(Guid venueId, CancellationToken ct = default);
    Task<BilliardTable?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<BilliardTable?> GetByIdWithDeviceAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<BilliardTable>> GetByStatusAsync(BilliardTableStatus status, CancellationToken ct = default);
    Task AddAsync(BilliardTable table, CancellationToken ct = default);
    void Update(BilliardTable table);
    void Remove(BilliardTable table);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
