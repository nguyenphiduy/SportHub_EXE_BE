namespace BidaPlatform.Application.Models.AI;

public class AiProviderSettingsResponse
{
    public string ProviderName { get; set; } = "OpenRouter";
    public string Model { get; set; } = "openrouter/free";
    public string BaseUrl { get; set; } = "https://openrouter.ai/api/v1";
    public bool IsEnabled { get; set; }
    public bool IsConfigured { get; set; }
    public string? MaskedApiKey { get; set; }
    public Guid? UpdatedByUserId { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
