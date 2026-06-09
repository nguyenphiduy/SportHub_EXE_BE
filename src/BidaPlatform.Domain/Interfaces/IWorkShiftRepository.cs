using BidaPlatform.Domain.Entities;

namespace BidaPlatform.Domain.Interfaces;

public interface IWorkShiftRepository
{
    Task AddAsync(WorkShift shift, CancellationToken ct = default);
    Task<WorkShift?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<WorkShift>> GetByVenueIdAsync(Guid venueId, CancellationToken ct = default);
    Task<List<WorkShift>> GetByStaffUserIdAsync(Guid staffUserId, CancellationToken ct = default);
    void Update(WorkShift shift);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
