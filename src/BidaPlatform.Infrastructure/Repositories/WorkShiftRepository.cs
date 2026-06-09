using BidaPlatform.Domain.Entities;
using BidaPlatform.Domain.Interfaces;
using BidaPlatform.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;

namespace BidaPlatform.Infrastructure.Repositories;

public class WorkShiftRepository : IWorkShiftRepository
{
    private readonly AppDbContext _db;

    public WorkShiftRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(WorkShift shift, CancellationToken ct = default)
        => await _db.WorkShifts.AddAsync(shift, ct);

    public Task<WorkShift?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.WorkShifts.FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<List<WorkShift>> GetByVenueIdAsync(Guid venueId, CancellationToken ct = default)
        => _db.WorkShifts.Where(x => x.VenueId == venueId)
            .OrderByDescending(x => x.ShiftDate)
            .ToListAsync(ct);

    public Task<List<WorkShift>> GetByStaffUserIdAsync(Guid staffUserId, CancellationToken ct = default)
        => _db.WorkShifts.Where(x => x.StaffUserId == staffUserId)
            .OrderByDescending(x => x.ShiftDate)
            .ToListAsync(ct);

    public void Update(WorkShift shift)
        => _db.WorkShifts.Update(shift);

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var shift = await _db.WorkShifts.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (shift != null)
        {
            _db.WorkShifts.Remove(shift);
        }
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}
