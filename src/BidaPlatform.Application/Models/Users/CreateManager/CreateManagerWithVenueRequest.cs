namespace BidaPlatform.Application.Models.Users.CreateManager;

public class CreateManagerWithVenueRequest
{
    // Manager info
    public string Email { get; set; } = null!;
    public string FullName { get; set; } = null!;

    // Venue info
    public string VenueName { get; set; } = null!;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? OwnerName { get; set; }
}
