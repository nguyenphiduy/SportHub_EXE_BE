using BidaPlatform.Domain.Entities;
using BidaPlatform.Domain.Enums;

namespace BidaPlatform.Domain.Interfaces;

public interface IBilliardSessionRepository
{
    Task<IEnumerable<BilliardSession>> GetAllAsync(CancellationToken ct = default);
    Task<IEnumerable<BilliardSession>> GetByTableIdAsync(Guid tableId, CancellationToken ct = default);
    Task<BilliardSession?> GetActiveSessionByTableIdAsync(Guid tableId, CancellationToken ct = default);
    Task<BilliardSession?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(BilliardSession session, CancellationToken ct = default);
    void Update(BilliardSession session);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
