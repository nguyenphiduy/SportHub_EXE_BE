namespace BidaPlatform.Application.Models.AI;

public class UpdateAiProviderSettingsRequest
{
    public string ApiKey { get; set; } = null!;
    public string Model { get; set; } = "openrouter/free";
    public string BaseUrl { get; set; } = "https://openrouter.ai/api/v1";
    public bool IsEnabled { get; set; } = true;
}
