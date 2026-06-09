using System.Text;
using System.Text.Json;
using BidaPlatform.Application.Interfaces;
using BidaPlatform.Application.Models.AI;

namespace BidaPlatform.Infrastructure.Services;

public class OpenRouterRequest
{
    public string model { get; set; } = null!;
    public List<OpenRouterMessage> messages { get; set; } = [];
    public float temperature { get; set; } = 0.3F;
}

public class OpenRouterMessage
{
    public string role { get; set; } = "user";
    public string content { get; set; } = null!;
}

public class OpenRouterResponse
{
    public List<OpenRouterChoice>? choices { get; set; }
    public OpenRouterUsage? usage { get; set; }
    public string? error { get; set; }
}

public class OpenRouterChoice
{
    public OpenRouterMessage? message { get; set; }
}

public class OpenRouterUsage
{
    public int prompt_tokens { get; set; }
    public int completion_tokens { get; set; }
    public int total_tokens { get; set; }
}

public class OpenRouterAiClient
{
    private readonly HttpClient _httpClient;
    private readonly IAiProviderSettingsService _settingsService;

    public OpenRouterAiClient(HttpClient httpClient, IAiProviderSettingsService settingsService)
    {
        _httpClient = httpClient;
        _settingsService = settingsService;
    }

    public async Task<AiInsightResponse?> GenerateInsightAsync(
        string prompt,
        CancellationToken ct = default)
    {
        var settings = await _settingsService.GetResolvedSettingsAsync(ct);
        if (settings == null)
            return null;

        return await CallOpenRouterAsync(settings.ApiKey, settings.Model, settings.BaseUrl, prompt, ct);
    }

    private async Task<AiInsightResponse?> CallOpenRouterAsync(
        string apiKey,
        string model,
        string baseUrl,
        string prompt,
        CancellationToken ct)
    {
        var requestBody = new OpenRouterRequest
        {
            model = model,
            messages =
            [
                new OpenRouterMessage
                {
                    role = "user",
                    content = prompt
                }
            ],
            temperature = 0.3F
        };

        var json = JsonSerializer.Serialize(requestBody);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl.TrimEnd('/')}/chat/completions");
        request.Headers.Add("Authorization", $"Bearer {apiKey}");
        request.Headers.Add("HTTP-Referer", "https://sporthub.local");
        request.Headers.Add("X-Title", "SportHub BidaPlatform");
        request.Content = content;

        HttpResponseMessage response;
        try
        {
            response = await _httpClient.SendAsync(request, ct);
        }
        catch (HttpRequestException)
        {
            return null;
        }
        catch (TaskCanceledException)
        {
            return null;
        }

        if (!response.IsSuccessStatusCode)
            return null;

        var responseBody = await response.Content.ReadAsStringAsync(ct);

        try
        {
            var openRouterResp = JsonSerializer.Deserialize<OpenRouterResponse>(responseBody);
            var contentText = openRouterResp?.choices?.FirstOrDefault()?.message?.content;

            if (string.IsNullOrWhiteSpace(contentText))
                return null;

            return ParseAiInsightResponse(contentText);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private AiInsightResponse? ParseAiInsightResponse(string rawContent)
    {
        var lines = rawContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        string? summary = null;
        string? trend = null;
        string? recommendation = null;
        decimal? estimated = null;

        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (trimmed.StartsWith("summary:", StringComparison.OrdinalIgnoreCase))
                summary = trimmed["summary:".Length..].Trim();
            else if (trimmed.StartsWith("trend:", StringComparison.OrdinalIgnoreCase))
                trend = trimmed["trend:".Length..].Trim();
            else if (trimmed.StartsWith("recommendation:", StringComparison.OrdinalIgnoreCase))
                recommendation = trimmed["recommendation:".Length..].Trim();
            else if (trimmed.StartsWith("estimated:", StringComparison.OrdinalIgnoreCase))
            {
                var valStr = trimmed["estimated:".Length..].Trim()
                    .Replace(".", "").Replace(",", "").Replace("VND", "").Trim();
                if (decimal.TryParse(valStr, out var val))
                    estimated = val;
            }
        }

        if (summary == null && trend == null && recommendation == null)
        {
            var jsonMatch = System.Text.RegularExpressions.Regex.Match(
                rawContent, @"\{.*\}", System.Text.RegularExpressions.RegexOptions.Singleline);
            if (jsonMatch.Success)
            {
                try
                {
                    var parsed = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonMatch.Value);
                    if (parsed == null) return null;
                    parsed.TryGetValue("summary", out var s);
                    parsed.TryGetValue("trend", out var t);
                    parsed.TryGetValue("recommendation", out var r);
                    parsed.TryGetValue("estimatedNextPeriodRevenue", out var e);

                    summary = s.GetString();
                    trend = t.GetString();
                    recommendation = r.GetString();
                    if (e.ValueKind == JsonValueKind.Number)
                        estimated = e.GetDecimal();
                }
                catch { }
            }
        }

        if (summary == null)
            return null;

        return new AiInsightResponse
        {
            Summary = summary ?? "",
            Trend = trend ?? "Không có đủ dữ liệu để phân tích xu hướng.",
            Recommendation = recommendation ?? "Tiếp tục theo dõi và tối ưu doanh thu.",
            EstimatedNextPeriodRevenue = estimated ?? 0
        };
    }
}
