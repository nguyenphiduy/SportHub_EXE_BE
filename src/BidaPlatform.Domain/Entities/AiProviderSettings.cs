namespace BidaPlatform.Domain.Entities;

/// <summary>
/// Stores encrypted OpenRouter API key and provider configuration.
/// There should be only one record for the entire system.
/// </summary>
public class AiProviderSettings
{
    public Guid Id { get; set; }

    /// <summary>Always "OpenRouter" for now.</summary>
    public string ProviderName { get; set; } = "OpenRouter";

    /// <summary>AES-256 encrypted API key. Decrypted only in-memory when calling OpenRouter.</summary>
    public string EncryptedApiKey { get; set; } = null!;

    /// <summary>
    /// OpenRouter model to use. Default is "openrouter/free".
    /// </summary>
    public string Model { get; set; } = "openrouter/free";

    /// <summary>
    /// OpenRouter base URL.
    /// </summary>
    public string BaseUrl { get; set; } = "https://openrouter.ai/api/v1";

    /// <summary>
    /// Whether AI insights are enabled system-wide.
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>SuperAdmin who last updated the settings.</summary>
    public Guid? UpdatedByUserId { get; set; }

    /// <summary>When the settings were last updated.</summary>
    public DateTime UpdatedAt { get; set; }

    public User? UpdatedByUser { get; set; }
}
