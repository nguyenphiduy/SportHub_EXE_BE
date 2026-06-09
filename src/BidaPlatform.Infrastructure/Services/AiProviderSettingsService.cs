using BidaPlatform.Application.Interfaces;
using BidaPlatform.Application.Models.AI;
using BidaPlatform.Domain.Entities;
using BidaPlatform.Domain.Interfaces;
using BidaPlatform.Infrastructure.Security;

namespace BidaPlatform.Infrastructure.Services;

public class AiProviderSettingsService : IAiProviderSettingsService
{
    private readonly IAiProviderSettingsRepository _repository;

    public AiProviderSettingsService(IAiProviderSettingsRepository repository)
    {
        _repository = repository;
    }

    public async Task<AiProviderSettingsResponse> GetSettingsAsync(CancellationToken ct = default)
    {
        var settings = await _repository.GetAsync(ct);
        return MapToResponse(settings);
    }

    public async Task<AiProviderSettingsResponse> UpdateSettingsAsync(
        Guid actorUserId,
        UpdateAiProviderSettingsRequest request,
        CancellationToken ct = default)
    {
        var settings = await _repository.GetAsync(ct);

        if (settings == null)
        {
            settings = new AiProviderSettings
            {
                Id = Guid.NewGuid(),
                ProviderName = "OpenRouter",
                EncryptedApiKey = EncryptionHelper.Encrypt(request.ApiKey),
                Model = string.IsNullOrWhiteSpace(request.Model) ? "openrouter/free" : request.Model,
                BaseUrl = string.IsNullOrWhiteSpace(request.BaseUrl) ? "https://openrouter.ai/api/v1" : request.BaseUrl,
                IsEnabled = request.IsEnabled,
                UpdatedByUserId = actorUserId,
                UpdatedAt = DateTime.UtcNow
            };
            await _repository.AddAsync(settings, ct);
        }
        else
        {
            settings.EncryptedApiKey = EncryptionHelper.Encrypt(request.ApiKey);
            settings.Model = string.IsNullOrWhiteSpace(request.Model) ? "openrouter/free" : request.Model;
            settings.BaseUrl = string.IsNullOrWhiteSpace(request.BaseUrl) ? "https://openrouter.ai/api/v1" : request.BaseUrl;
            settings.IsEnabled = request.IsEnabled;
            settings.UpdatedByUserId = actorUserId;
            settings.UpdatedAt = DateTime.UtcNow;
            _repository.Update(settings);
        }

        await _repository.SaveChangesAsync(ct);
        return MapToResponse(settings);
    }

    public async Task<ResolvedAiProviderSettings?> GetResolvedSettingsAsync(CancellationToken ct = default)
    {
        var settings = await _repository.GetAsync(ct);
        if (settings == null || !settings.IsEnabled || string.IsNullOrWhiteSpace(settings.EncryptedApiKey))
            return null;

        var decryptedKey = EncryptionHelper.Decrypt(settings.EncryptedApiKey);
        if (string.IsNullOrWhiteSpace(decryptedKey))
            return null;

        return new ResolvedAiProviderSettings
        {
            ProviderName = settings.ProviderName,
            ApiKey = decryptedKey,
            Model = settings.Model,
            BaseUrl = settings.BaseUrl,
            IsEnabled = settings.IsEnabled
        };
    }

    private static AiProviderSettingsResponse MapToResponse(AiProviderSettings? settings)
    {
        if (settings == null)
        {
            return new AiProviderSettingsResponse
            {
                ProviderName = "OpenRouter",
                Model = "openrouter/free",
                BaseUrl = "https://openrouter.ai/api/v1",
                IsEnabled = false,
                IsConfigured = false,
                MaskedApiKey = null,
                UpdatedByUserId = null,
                UpdatedAt = null
            };
        }

        return new AiProviderSettingsResponse
        {
            ProviderName = settings.ProviderName,
            Model = settings.Model,
            BaseUrl = settings.BaseUrl,
            IsEnabled = settings.IsEnabled,
            IsConfigured = !string.IsNullOrWhiteSpace(settings.EncryptedApiKey),
            MaskedApiKey = MaskApiKey(EncryptionHelper.Decrypt(settings.EncryptedApiKey)),
            UpdatedByUserId = settings.UpdatedByUserId,
            UpdatedAt = settings.UpdatedAt
        };
    }

    private static string? MaskApiKey(string? key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return null;
        if (key.Length <= 8)
            return "****";
        return key[..8] + "****" + key[^4..];
    }
}
