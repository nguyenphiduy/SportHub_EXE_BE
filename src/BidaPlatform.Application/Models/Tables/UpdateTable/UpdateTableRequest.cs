namespace BidaPlatform.Application.Models.Tables.UpdateTable;

public class UpdateTableRequest
{
    public string? Name { get; set; }
    public string? Type { get; set; }
    public decimal? PricePerHour { get; set; }
    public string? DeviceIpAddress { get; set; }
    public string? DeviceName { get; set; }
}
