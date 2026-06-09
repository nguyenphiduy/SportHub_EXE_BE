using BidaPlatform.Domain.Entities;
using BidaPlatform.Domain.Interfaces;
using BidaPlatform.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;

namespace BidaPlatform.Infrastructure.Repositories;

public class VenueRepository : IVenueRepository
{
    private readonly AppDbContext _db;

    public VenueRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(Venue venue, CancellationToken ct = default)
        => await _db.Venues.AddAsync(venue, ct);

    public Task<Venue?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.Venues.FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<Venue?> GetByIdWithManagerAsync(Guid id, CancellationToken ct = default)
        => _db.Venues
            .Include(x => x.PrimaryManager)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<List<Venue>> GetAllAsync(CancellationToken ct = default)
        => _db.Venues
            .Include(x => x.PrimaryManager)
            .Include(x => x.Subscriptions)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);

    public Task<List<Venue>> GetByManagerIdAsync(Guid managerId, CancellationToken ct = default)
        => _db.Venues
            .Include(x => x.PrimaryManager)
            .Where(x => x.PrimaryManagerId == managerId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);

    public void Update(Venue venue)
        => _db.Venues.Update(venue);

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}
