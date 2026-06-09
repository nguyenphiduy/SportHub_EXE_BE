using MediatR;
using BidaPlatform.Application.Models.Users.CreateManager;

namespace BidaPlatform.Application.UseCases.Users.CreateManager;

public class CreateManagerWithVenueCommand : IRequest
{
    public string Email { get; }
    public string FullName { get; }
    public string VenueName { get; }
    public string? Address { get; }
    public string? Phone { get; }
    public string? OwnerName { get; }

    public CreateManagerWithVenueCommand(string email, string fullName, string venueName, string? address, string? phone, string? ownerName)
    {
        Email = email;
        FullName = fullName;
        VenueName = venueName;
        Address = address;
        Phone = phone;
        OwnerName = ownerName;
    }
}
