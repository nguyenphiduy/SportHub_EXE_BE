using BidaPlatform.Domain.Entities;
using BidaPlatform.Domain.Enums;
using BidaPlatform.Domain.Interfaces;
using BidaPlatform.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;

namespace BidaPlatform.Infrastructure.Repositories;

public class BilliardTableRepository : IBilliardTableRepository
{
    private readonly AppDbContext _db;

    public BilliardTableRepository(AppDbContext db) => _db = db;

    public async Task<IEnumerable<BilliardTable>> GetAllAsync(CancellationToken ct = default)
        => await _db.BilliardTables
            .Include(x => x.IoTDevice)
            .OrderBy(x => x.Name)
            .ToListAsync(ct);

    public async Task<IEnumerable<BilliardTable>> GetActiveAsync(CancellationToken ct = default)
        => await _db.BilliardTables
            .Where(x => x.IsActive)
            .Include(x => x.IoTDevice)
            .OrderBy(x => x.Name)
            .ToListAsync(ct);

    public async Task<IEnumerable<BilliardTable>> GetAllByVenueAsync(Guid venueId, CancellationToken ct = default)
        => await _db.BilliardTables
            .Where(x => x.VenueId == venueId)
            .Include(x => x.IoTDevice)
            .OrderBy(x => x.Name)
            .ToListAsync(ct);

    public async Task<IEnumerable<BilliardTable>> GetActiveByVenueAsync(Guid venueId, CancellationToken ct = default)
        => await _db.BilliardTables
            .Where(x => x.VenueId == venueId && x.IsActive)
            .Include(x => x.IoTDevice)
            .OrderBy(x => x.Name)
            .ToListAsync(ct);

    public async Task<BilliardTable?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.BilliardTables
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<BilliardTable?> GetByIdWithDeviceAsync(Guid id, CancellationToken ct = default)
        => await _db.BilliardTables
            .Include(x => x.IoTDevice)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<IEnumerable<BilliardTable>> GetByStatusAsync(BilliardTableStatus status, CancellationToken ct = default)
        => await _db.BilliardTables
            .Where(x => x.Status == status && x.IsActive)
            .Include(x => x.IoTDevice)
            .ToListAsync(ct);

    public async Task AddAsync(BilliardTable table, CancellationToken ct = default)
        => await _db.BilliardTables.AddAsync(table, ct);

    public void Update(BilliardTable table)
        => _db.BilliardTables.Update(table);

    public void Remove(BilliardTable table)
        => _db.BilliardTables.Remove(table);

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}
