using BidaPlatform.Domain.Entities;

namespace BidaPlatform.Domain.Interfaces;

public interface IAiProviderSettingsRepository
{
    /// <summary>
    /// Returns the single system-wide AI provider settings record.
    /// Returns null if not yet configured.
    /// </summary>
    Task<AiProviderSettings?> GetAsync(CancellationToken ct = default);

    /// <summary>
    /// Creates a new settings record. Should only be called once on first configuration.
    /// </summary>
    Task AddAsync(AiProviderSettings settings, CancellationToken ct = default);

    /// <summary>
    /// Updates the existing settings record.
    /// </summary>
    void Update(AiProviderSettings settings);

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
