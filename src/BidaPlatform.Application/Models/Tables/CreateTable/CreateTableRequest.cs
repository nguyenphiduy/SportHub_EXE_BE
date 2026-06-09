namespace BidaPlatform.Application.Models.Tables.CreateTable;

public class CreateTableRequest
{
    public string Name { get; set; } = null!;
    public string Type { get; set; } = null!;       // Standard | VIP | Premium
    public decimal PricePerHour { get; set; }
    public string? DeviceIpAddress { get; set; }    // IP của ESP8266, tuỳ chọn
    public string? DeviceName { get; set; }
}
