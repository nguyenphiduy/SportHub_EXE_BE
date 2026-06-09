using BidaPlatform.Domain.Entities;
using BidaPlatform.Domain.Interfaces;
using BidaPlatform.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;

namespace BidaPlatform.Infrastructure.Repositories;

public class AuthTokenRepository : IAuthTokenRepository
{
    private readonly AppDbContext _db;
    public AuthTokenRepository(AppDbContext db) => _db = db;

    public Task<AuthToken?> GetByTokenAsync(string token)
        => _db.AuthTokens.FirstOrDefaultAsync(x => x.AccessToken == token);

    public async Task<AuthToken?> GetRefreshTokenAsync(
    string token,
    CancellationToken ct = default)
    {
        return await _db.AuthTokens
            .Include(t => t.User)
            .ThenInclude(u => u.AuthTokens)
            .FirstOrDefaultAsync(t =>
                t.RefreshToken == token &&
                !t.IsRevoked,
                ct);
    }


    public Task<AuthToken?> GetAccessTokenByUserIdAsync(Guid userId, CancellationToken ct = default)
        => _db.AuthTokens.FirstOrDefaultAsync(x => x.UserId == userId && !x.IsRevoked, ct);

    public async Task SaveTokenAsync(AuthToken token, CancellationToken ct = default)
        => await _db.AuthTokens.AddAsync(token, ct);

    public async Task SaveResetTokenAsync(AuthToken token, CancellationToken ct = default)
        => await _db.AuthTokens.AddAsync(token, ct);

    public async Task DeleteAsync(string refreshToken)
    {
        var token = await _db.AuthTokens.FirstOrDefaultAsync(x => x.RefreshToken == refreshToken);
        if (token != null) _db.AuthTokens.Remove(token);
    }

    public async Task RemoveTokenAsync(AuthToken token, CancellationToken ct = default)
        => _db.AuthTokens.Remove(token);

    public async Task UpdateAsync(AuthToken token, CancellationToken ct = default)
    {
        _db.AuthTokens.Update(token);
        await SaveChangeAsync(ct);
    }

    public void Update(AuthToken token)
        => _db.AuthTokens.Update(token);

    public Task SaveChangeAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);

    public async Task<int> DeleteRevokedOlderThanAsync(DateTime cutoffUtc, CancellationToken ct = default)
    {
        return await _db.AuthTokens
            .Where(x =>
                x.IsRevoked &&
                x.CreatedAt <= cutoffUtc)
            .ExecuteDeleteAsync(ct);
    }

    public async Task<int> DeleteExpiredOrRevokedAsync(
    DateTime revokedCutoffUtc,
    DateTime nowUtc,
    CancellationToken ct = default)
    {
        return await _db.AuthTokens
            .Where(x =>
                // 🔥 Case 1: revoked lâu hơn TTL
                (x.IsRevoked && x.CreatedAt <= revokedCutoffUtc)

                // 🔥 Case 2: refresh token hết hạn
                || x.RefreshTokenExpiresAt <= nowUtc
            )
            .ExecuteDeleteAsync(ct);
    }


}


