using BidaPlatform.Domain.Entities;

namespace BidaPlatform.Domain.Interfaces;

public interface IVenueRepository
{
    Task AddAsync(Venue venue, CancellationToken ct = default);
    Task<Venue?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Venue?> GetByIdWithManagerAsync(Guid id, CancellationToken ct = default);
    Task<List<Venue>> GetAllAsync(CancellationToken ct = default);
    Task<List<Venue>> GetByManagerIdAsync(Guid managerId, CancellationToken ct = default);
    void Update(Venue venue);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
