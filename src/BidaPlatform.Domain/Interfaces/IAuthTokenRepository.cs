using BidaPlatform.Domain.Entities;

namespace BidaPlatform.Domain.Interfaces;

/// <summary>
/// Defines repository operations for authentication tokens (Access / Refresh).
/// </summary>
public interface IAuthTokenRepository
{
    /// <summary>
    /// Gets token by access token string.
    /// </summary>
    Task<AuthToken?> GetByTokenAsync(string token);

    /// <summary>
    /// Gets refresh token by token string.
    /// </summary>
    Task<AuthToken?> GetRefreshTokenAsync(string token, CancellationToken ct = default);

    /// <summary>
    /// Gets active access token by user id.
    /// </summary>
    Task<AuthToken?> GetAccessTokenByUserIdAsync(Guid userId, CancellationToken ct = default);

    /// <summary>
    /// Saves a new access / refresh token.
    /// </summary>
    Task SaveTokenAsync(AuthToken token, CancellationToken ct = default);

    /// <summary>
    /// Saves reset-password token (reuse AuthToken entity).
    /// </summary>
    Task SaveResetTokenAsync(AuthToken token, CancellationToken ct = default);

    /// <summary>
    /// Deletes token by refresh token string.
    /// </summary>
    Task DeleteAsync(string refreshToken);

    /// <summary>
    /// Removes token entity.
    /// </summary>
    Task RemoveTokenAsync(AuthToken token, CancellationToken ct = default);

    /// <summary>
    /// Updates token entity and saves immediately.
    /// </summary>
    Task UpdateAsync(AuthToken token, CancellationToken ct = default);

    /// <summary>
    /// Updates token entity without saving (batch / transaction).
    /// </summary>
    void Update(AuthToken token);

    /// <summary>
    /// Saves database changes.
    /// </summary>
    Task SaveChangeAsync(CancellationToken ct = default);

    Task<int> DeleteRevokedOlderThanAsync(DateTime cutoffUtc, CancellationToken ct = default);
    Task<int> DeleteExpiredOrRevokedAsync(DateTime revokedCutoffUtc, DateTime nowUtc, CancellationToken ct = default);

}
