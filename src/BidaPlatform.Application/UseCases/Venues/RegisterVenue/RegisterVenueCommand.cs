using MediatR;
using BidaPlatform.Application.Models.Venues.Common;
using BidaPlatform.Application.Models.Venues.RegisterVenue;
using BidaPlatform.Domain.Entities;
using BidaPlatform.Domain.Enums;
using BidaPlatform.Domain.Interfaces;
using BidaPlatform.Application.Interfaces;

namespace BidaPlatform.Application.UseCases.Venues.RegisterVenue;

public record RegisterVenueCommand(RegisterVenueRequest Request) : IRequest<VenueResponse>;

public class RegisterVenueHandler : IRequestHandler<RegisterVenueCommand, VenueResponse>
{
    private readonly IVenueRepository _venueRepository;
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly INotificationBroadcaster _notificationBroadcaster;

    public RegisterVenueHandler(
        IVenueRepository venueRepository,
        IUserRepository userRepository,
        IEmailService emailService,
        INotificationBroadcaster notificationBroadcaster)
    {
        _venueRepository = venueRepository;
        _userRepository = userRepository;
        _emailService = emailService;
        _notificationBroadcaster = notificationBroadcaster;
    }

    public async Task<VenueResponse> Handle(RegisterVenueCommand request, CancellationToken ct)
    {
        var managerExists = await _userRepository.GetByEmailWithoutDecryptAsync(request.Request.ManagerEmail, ct);
        if (managerExists != null)
            throw new InvalidOperationException("Email manager đã tồn tại");

        var tempPassword = GenerateRandomPassword(10);

        var manager = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Request.ManagerEmail,
            Password = BCrypt.Net.BCrypt.HashPassword(tempPassword),
            FullName = request.Request.ManagerFullName,
            Role = UserRole.Manager.ToString(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(manager, ct);
        await _userRepository.SaveChangesAsync(ct);

        var venue = new Venue
        {
            Id = Guid.NewGuid(),
            Name = request.Request.Name,
            Address = request.Request.Address,
            Phone = request.Request.Phone,
            OwnerName = request.Request.OwnerName,
            Status = VenueStatus.Pending,
            IsActive = true,
            PrimaryManagerId = manager.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        manager.VenueId = venue.Id;
        await _venueRepository.AddAsync(venue, ct);
        _userRepository.Update(manager);
        await _venueRepository.SaveChangesAsync(ct);

        await _notificationBroadcaster.BroadcastAsync("venue", "registered");
        await _emailService.SendAsync(
            request.Request.ManagerEmail,
            "Dang ky quan thanh cong",
            $"Xin chao {request.Request.ManagerFullName},\n\nQuan '{request.Request.Name}' da duoc dang ky va dang cho SuperAdmin phe duyet.\n\nThong tin dang nhap tam thoi:\n- Email: {request.Request.ManagerEmail}\n- Mat khau: {tempPassword}\n\nVui long dang nhap va doi mat khau ngay sau khi tai khoan duoc kich hoat.");

        return new VenueResponse
        {
            Id = venue.Id,
            Name = venue.Name,
            Address = venue.Address,
            Phone = venue.Phone,
            OwnerName = venue.OwnerName,
            Status = venue.Status.ToString(),
            IsActive = venue.IsActive,
            PrimaryManagerId = venue.PrimaryManagerId,
            PrimaryManagerName = manager.FullName,
            CreatedAt = venue.CreatedAt,
            UpdatedAt = venue.UpdatedAt
        };
    }

    private static string GenerateRandomPassword(int length)
    {
        const string chars =
            "ABCDEFGHJKLMNPQRSTUVWXYZ" +
            "abcdefghijkmnopqrstuvwxyz" +
            "23456789!@#$%";

        var bytes = System.Security.Cryptography.RandomNumberGenerator.GetBytes(length);
        var result = new char[length];

        for (int i = 0; i < length; i++)
            result[i] = chars[bytes[i] % chars.Length];

        return new string(result);
    }
}
