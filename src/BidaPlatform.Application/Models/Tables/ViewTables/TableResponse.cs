namespace BidaPlatform.Application.Models.Tables.ViewTables;

public class TableResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Type { get; set; } = null!;
    public decimal PricePerHour { get; set; }
    public string Status { get; set; } = null!;
    public bool IsActive { get; set; }
    public IoTDeviceInfo? Device { get; set; }
}

public class IoTDeviceInfo
{
    public Guid Id { get; set; }
    public string IpAddress { get; set; } = null!;
    public string? DeviceName { get; set; }
    public bool IsOnline { get; set; }
}
