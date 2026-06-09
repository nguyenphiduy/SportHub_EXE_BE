using BidaPlatform.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BidaPlatform.Infrastructure.Jobs;

/// <summary>
/// Background job ping thiết bị IoT định kỳ để ghi log theo dõi.
/// Không cập nhật IsOnline trong DB: trạng thái online/offline chỉ đổi khi gọi API quét thủ công.
/// </summary>
public class IoTHealthCheckJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<IoTHealthCheckJob> _logger;
    private readonly bool _iotEnabled;
    private readonly TimeSpan _pingInterval;

    public IoTHealthCheckJob(
        IServiceScopeFactory scopeFactory,
        ILogger<IoTHealthCheckJob> logger,
        IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _iotEnabled = configuration.GetValue<bool>("IoT:Enabled", defaultValue: true);
        var intervalSeconds = configuration.GetValue<int>("IoT:PingIntervalSeconds", defaultValue: 120);
        _pingInterval = TimeSpan.FromSeconds(intervalSeconds);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_iotEnabled)
        {
            _logger.LogInformation("IoTHealthCheckJob: IoT disabled — job không chạy.");
            return;
        }

        // Ping ngay khi khởi động để có log ban đầu
        await PingAllDevicesAsync(stoppingToken);

        using var timer = new PeriodicTimer(_pingInterval);
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await PingAllDevicesAsync(stoppingToken);
        }
    }

    private async Task PingAllDevicesAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var deviceRepo = scope.ServiceProvider.GetRequiredService<IIoTDeviceRepository>();
        var iotService = scope.ServiceProvider.GetRequiredService<IIoTControlService>();

        List<Domain.Entities.IoTDevice> devices;
        try
        {
            devices = await deviceRepo.GetAllAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "IoTHealthCheckJob: Lỗi khi lấy danh sách thiết bị.");
            return;
        }

        if (devices.Count == 0) return;

        // Ping song song tất cả thiết bị để ghi log theo dõi, không ghi đè trạng thái DB.
        var tasks = devices.Select(async device =>
        {
            var isOnline = await iotService.PingAsync(device.IpAddress, ct);

            _logger.LogInformation(
                "IoTHealthCheckJob: Ping {Name} ({Ip}) => {Status}",
                device.DeviceName ?? device.Id.ToString(),
                device.IpAddress,
                isOnline ? "ONLINE" : "OFFLINE");
        });

        await Task.WhenAll(tasks);
    }
}
