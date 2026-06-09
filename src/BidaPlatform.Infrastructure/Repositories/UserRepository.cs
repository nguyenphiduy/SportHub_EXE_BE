using BidaPlatform.Domain.Entities;
using BidaPlatform.Domain.Interfaces;
using BidaPlatform.Infrastructure.Identity;
using BidaPlatform.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;

namespace BidaPlatform.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;
    public UserRepository(AppDbContext db) => _db = db;

    public async Task<IEnumerable<User>> GetListUser()
    {
        var users = await _db.Users
            .AsNoTracking()
            .ToListAsync();

        users.ForEach(Decrypt);
        return users;
    }

    public async Task<IEnumerable<User>> GetListUserWithVenue()
    {
        var users = await _db.Users
            .AsNoTracking()
            .Include(x => x.Venue)
            .ToListAsync();

        users.ForEach(Decrypt);
        return users;
    }

    public async Task<User?> GetByEmailAsync(
        string email,
        CancellationToken ct = default)
    {
        var encrypted = EncryptionHelper.EncryptDeterministic(email);

        var user = await _db.Users
            .AsNoTracking()
            .Include(x => x.AuthTokens)
            .FirstOrDefaultAsync(x =>
                x.Email == encrypted &&
                x.IsActive,
                ct);

        if (user != null) Decrypt(user);
        return user;
    }

    public async Task<User?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default)
    {
        var user = await _db.Users
            .AsNoTracking()
            .Include(x => x.AuthTokens)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (user != null) Decrypt(user);
        return user;
    }

    public async Task AddAsync(User user, CancellationToken ct = default)
    {
        Encrypt(user);
        await _db.Users.AddAsync(user, ct);
    }

    public void Update(User user)
    {
        Encrypt(user);
        _db.Users.Update(user);
    }

    public void UpdateV1(User user) => _db.Users.Update(user);

    public void UpdatePasswordOnly(User user)
    {
        var entry = _db.Entry(user);
        entry.Property(x => x.Password).IsModified = true;
    }

    public void Remove(User user) => _db.Users.Remove(user);

    public async Task<List<User>> GetUsersByRoleAsync(string role, CancellationToken ct = default)
        => await _db.Users
            .AsNoTracking()
            .Where(x => x.Role == role && x.IsActive)
            .ToListAsync(ct);

    public async Task<List<User>> GetUsersByVenueAsync(Guid venueId, CancellationToken ct = default)
    {
        var users = await _db.Users
            .AsNoTracking()
            .Where(x => x.VenueId == venueId && x.IsActive)
            .ToListAsync(ct);

        users.ForEach(Decrypt);
        return users;
    }

    public Task<User?> GetByIdWithoutDecryptAsync(Guid id, CancellationToken ct = default)
        => _db.Users.FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<User?> GetByEmailWithoutDecryptAsync(string email, CancellationToken ct = default)
    {
        var encrypted = EncryptionHelper.EncryptDeterministic(email);
        return _db.Users.FirstOrDefaultAsync(
            x => x.Email == encrypted && x.IsActive,
            ct);
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);

    public void Deactivate(User user)
    {
        user.IsActive = false;
        _db.Users.Update(user);
    }

    public void Activate(User user)
    {
        user.IsActive = true;
        _db.Users.Update(user);
    }

    private static void Encrypt(User u)
    {
        u.Email = EncryptionHelper.EncryptDeterministic(u.Email);
        u.FullName = EncryptionHelper.Encrypt(u.FullName);
    }

    private static void Decrypt(User u)
    {
        u.Email = EncryptionHelper.DecryptDeterministic(u.Email);
        u.FullName = EncryptionHelper.Decrypt(u.FullName);
    }
}
