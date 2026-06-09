using BidaPlatform.Domain.Entities;
using BidaPlatform.Domain.Enums;
using BidaPlatform.Domain.Interfaces;
using BidaPlatform.Infrastructure.Identity;
using BidaPlatform.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace BidaPlatform.Infrastructure.Repositories;

public class BilliardSessionRepository : IBilliardSessionRepository
{
    private readonly AppDbContext _db;

    public BilliardSessionRepository(AppDbContext db) => _db = db;

    public async Task<IEnumerable<BilliardSession>> GetAllAsync(CancellationToken ct = default)
    {
        var sessions = await _db.BilliardSessions
            .Include(x => x.Table)
            .Include(x => x.StartedByUser)
            .OrderByDescending(x => x.StartTime)
            .ToListAsync(ct);

        foreach (var s in sessions)
            DecryptUser(s.StartedByUser);

        return sessions;
    }

    public async Task<IEnumerable<BilliardSession>> GetByTableIdAsync(Guid tableId, CancellationToken ct = default)
    {
        var sessions = await _db.BilliardSessions
            .Where(x => x.TableId == tableId)
            .Include(x => x.StartedByUser)
            .OrderByDescending(x => x.StartTime)
            .ToListAsync(ct);

        foreach (var s in sessions)
            DecryptUser(s.StartedByUser);

        return sessions;
    }

    public async Task<BilliardSession?> GetActiveSessionByTableIdAsync(Guid tableId, CancellationToken ct = default)
        => await _db.BilliardSessions
            .FirstOrDefaultAsync(x => x.TableId == tableId && x.Status == BilliardSessionStatus.Active, ct);

    public async Task<BilliardSession?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.BilliardSessions
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task AddAsync(BilliardSession session, CancellationToken ct = default)
        => await _db.BilliardSessions.AddAsync(session, ct);

    public void Update(BilliardSession session)
        => _db.BilliardSessions.Update(session);

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);

    private static void DecryptUser(User? user)
    {
        if (user is null) return;
        if (!string.IsNullOrWhiteSpace(user.FullName))
            try { user.FullName = EncryptionHelper.Decrypt(user.FullName); }
            catch (FormatException) { /* already plain text – leave as-is */ }
            catch (CryptographicException) { /* encrypted with old/different key – leave as-is */ }
    }
}
