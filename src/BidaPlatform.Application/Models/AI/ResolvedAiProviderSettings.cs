namespace BidaPlatform.Application.Models.AI;

public class ResolvedAiProviderSettings
{
    public string ProviderName { get; set; } = "OpenRouter";
    public string ApiKey { get; set; } = null!;
    public string Model { get; set; } = "openrouter/free";
    public string BaseUrl { get; set; } = "https://openrouter.ai/api/v1";
    public bool IsEnabled { get; set; }
}
