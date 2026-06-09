namespace BidaPlatform.Application.Models.Users.ReadAllUser;

public class ReadAllUserResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Role { get; set; } = null!;
    public Guid? VenueId { get; set; }
    public string? VenueName { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
