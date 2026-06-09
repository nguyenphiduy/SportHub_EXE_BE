namespace BidaPlatform.Application.Models.Users.CreateUser;

public class CreateUserRequest
{
    public string Email { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public Guid VenueId { get; set; }
}
