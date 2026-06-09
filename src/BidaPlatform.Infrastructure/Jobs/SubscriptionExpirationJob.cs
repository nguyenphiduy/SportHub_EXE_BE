using BidaPlatform.Domain.Enums;
using BidaPlatform.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BidaPlatform.Infrastructure.Jobs;

public class SubscriptionExpirationJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SubscriptionExpirationJob> _logger;
    private static readonly TimeSpan CheckInterval = TimeSpan.FromMinutes(5);

    public SubscriptionExpirationJob(
        IServiceScopeFactory scopeFactory,
        ILogger<SubscriptionExpirationJob> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SubscriptionExpirationJob: Started — check interval {Interval} minutes", CheckInterval.TotalMinutes);

        using var timer = new PeriodicTimer(CheckInterval);
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await CheckAndExpireAsync(stoppingToken);
        }
    }

    private async Task CheckAndExpireAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IVenueSubscriptionRepository>();

        try
        {
            var now = DateTime.UtcNow;
            var expired = await repo.GetActiveExpiredAsync(now, ct);

            foreach (var sub in expired)
            {
                sub.Status = BidaPlatform.Domain.Enums.VenueSubscriptionStatus.Expired;
                sub.UpdatedAt = now;
                repo.Update(sub);
                _logger.LogInformation(
                    "SubscriptionExpirationJob: VenueId={VenueId} Plan={Plan} expired — status set to Expired",
                    sub.VenueId, sub.Plan);
            }

            if (expired.Count > 0)
            {
                await repo.SaveChangesAsync(ct);
                _logger.LogInformation("SubscriptionExpirationJob: Processed {Count} expired subscription(s)", expired.Count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SubscriptionExpirationJob: Error during expiration check");
        }
    }
}
