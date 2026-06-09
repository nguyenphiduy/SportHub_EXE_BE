using BidaPlatform.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BidaPlatform.Infrastructure.Services;

public class IoTControlService : IIoTControlService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<IoTControlService> _logger;
    private readonly bool _iotEnabled;
    // Timeout cho lệnh điều khiển: 5 giây — tránh treo API khi BE không cùng mạng LAN với thiết bị
    private static readonly TimeSpan CommandTimeout = TimeSpan.FromSeconds(5);

    public IoTControlService(
        IHttpClientFactory httpClientFactory,
        ILogger<IoTControlService> logger,
        IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _iotEnabled = configuration.GetValue<bool>("IoT:Enabled", defaultValue: true);
    }

    public async Task<bool> TurnOnAsync(string deviceIpAddress, CancellationToken ct = default)
        => await SendCommandAsync(deviceIpAddress, "on", ct);

    public async Task<bool> TurnOffAsync(string deviceIpAddress, CancellationToken ct = default)
        => await SendCommandAsync(deviceIpAddress, "off", ct);

    public async Task<bool> PingAsync(string deviceIpAddress, CancellationToken ct = default)
    {
        if (!_iotEnabled)
        {
            _logger.LogInformation("IoT disabled — skipping ping for {Ip}", deviceIpAddress);
            return false;
        }
        try
        {
            // Dùng timeout ngắn hơn (3s) để ping nhanh
            var client = _httpClientFactory.CreateClient("IoTClient");
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromSeconds(3));
            var response = await client.GetAsync($"http://{deviceIpAddress}/", cts.Token);
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            // Tránh false-positive từ router/modem: chỉ chấp nhận firmware ESP trả body "OK".
            var body = (await response.Content.ReadAsStringAsync(cts.Token)).Trim();
            return string.Equals(body, "OK", StringComparison.OrdinalIgnoreCase);
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex, "Ping failed for device {Ip}", deviceIpAddress);
            return false;
        }
    }

    private async Task<bool> SendCommandAsync(string deviceIpAddress, string command, CancellationToken ct)
    {
        if (!_iotEnabled)
        {
            _logger.LogInformation("IoT disabled — skipping command '{Command}' for {Ip}", command, deviceIpAddress);
            return false;
        }
        try
        {
            var client = _httpClientFactory.CreateClient("IoTClient");
            var url = $"http://{deviceIpAddress}/{command}";
            // Dùng timeout 5s: nếu BE không cùng LAN với thiết bị, API vẫn trả về nhanh thay vì treo
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(CommandTimeout);
            var response = await client.GetAsync(url, cts.Token);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "IoT command '{Command}' failed for device {Ip}", command, deviceIpAddress);
            return false;
        }
    }
}
