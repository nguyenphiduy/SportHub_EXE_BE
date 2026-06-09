namespace BidaPlatform.Domain.Interfaces;

public interface IIoTControlService
{
    /// <summary>Gửi lệnh BẬT đèn tới ESP8266</summary>
    Task<bool> TurnOnAsync(string deviceIpAddress, CancellationToken ct = default);

    /// <summary>Gửi lệnh TẮT đèn tới ESP8266</summary>
    Task<bool> TurnOffAsync(string deviceIpAddress, CancellationToken ct = default);

    /// <summary>Thử kết nối tới thiết bị, trả về true nếu thiết bị phản hồi.</summary>
    Task<bool> PingAsync(string deviceIpAddress, CancellationToken ct = default);
}
