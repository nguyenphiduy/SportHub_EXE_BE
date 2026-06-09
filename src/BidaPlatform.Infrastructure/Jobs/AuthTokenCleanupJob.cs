using BidaPlatform.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BidaPlatform.Infrastructure.Jobs;

/// <summary>
/// Cleanup revoked / expired auth tokens based on DB time (TTL-based)
/// </summary>
public class AuthTokenCleanupJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AuthTokenCleanupJob> _logger;

    // 🔥 TTL cho token đã revoke
    private static readonly TimeSpan RevokedTtl = TimeSpan.FromHours(6);

    // ⏱️ Chu kỳ chạy cleanup
    private static readonly TimeSpan RunInterval = TimeSpan.FromMinutes(30);

    public AuthTokenCleanupJob(
        IServiceScopeFactory scopeFactory,
        ILogger<AuthTokenCleanupJob> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // ✅ CLEAN NGAY KHI BE START
        await CleanupAsync(stoppingToken);

        using var timer = new PeriodicTimer(RunInterval);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await CleanupAsync(stoppingToken);
        }
    }

    private async Task CleanupAsync(CancellationToken ct)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider
                .GetRequiredService<IAuthTokenRepository>();

            var now = DateTime.UtcNow;
            var revokedCutoff = now - RevokedTtl;

            var deleted = await repo.DeleteExpiredOrRevokedAsync(
                revokedCutoff,
                now,
                ct);

            if (deleted > 0)
            {
                _logger.LogInformation(
                    "AuthTokenCleanupJob deleted {Count} expired/revoked tokens",
                    deleted);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AuthTokenCleanupJob failed");
        }
    }
}
