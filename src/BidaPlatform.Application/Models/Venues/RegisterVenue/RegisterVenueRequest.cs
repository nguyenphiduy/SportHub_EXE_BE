namespace BidaPlatform.Application.Models.Venues.RegisterVenue;

public class RegisterVenueRequest
{
    public string Name { get; set; } = null!;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string OwnerName { get; set; } = null!;
    public string ManagerEmail { get; set; } = null!;
    public string ManagerFullName { get; set; } = null!;
}
