using BidaPlatform.Application.Models.AI;

namespace BidaPlatform.Application.Interfaces;

public interface IAiProviderSettingsService
{
    Task<AiProviderSettingsResponse> GetSettingsAsync(CancellationToken ct = default);
    Task<AiProviderSettingsResponse> UpdateSettingsAsync(Guid actorUserId, UpdateAiProviderSettingsRequest request, CancellationToken ct = default);
    Task<ResolvedAiProviderSettings?> GetResolvedSettingsAsync(CancellationToken ct = default);
}
