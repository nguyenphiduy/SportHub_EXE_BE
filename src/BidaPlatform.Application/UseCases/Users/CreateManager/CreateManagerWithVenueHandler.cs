using BidaPlatform.Application.Models.Users.CreateManager;
using BidaPlatform.Domain.Entities;
using BidaPlatform.Domain.Enums;
using BidaPlatform.Domain.Interfaces;
using MediatR;
using BidaPlatform.Application.Interfaces;
using System.Security.Cryptography;

namespace BidaPlatform.Application.UseCases.Users.CreateManager;

public class CreateManagerWithVenueHandler : IRequestHandler<CreateManagerWithVenueCommand>
{
    private readonly IUserRepository _userRepo;
    private readonly IVenueRepository _venueRepo;
    private readonly IEmailService _emailService;

    public CreateManagerWithVenueHandler(IUserRepository userRepo, IVenueRepository venueRepo, IEmailService emailService)
    {
        _userRepo = userRepo;
        _venueRepo = venueRepo;
        _emailService = emailService;
    }

    public async Task Handle(CreateManagerWithVenueCommand request, CancellationToken ct)
    {
        var emailExists = await _userRepo.GetByEmailWithoutDecryptAsync(request.Email, ct);
        if (emailExists != null)
            throw new InvalidOperationException("Email da ton tai");

        var tempPassword = GenerateRandomPassword(10);

        var venue = new Venue
        {
            Id = Guid.NewGuid(),
            Name = request.VenueName,
            Address = request.Address,
            Phone = request.Phone,
            OwnerName = request.OwnerName,
            Status = VenueStatus.Pending,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _venueRepo.AddAsync(venue, ct);
        await _venueRepo.SaveChangesAsync(ct);

        var manager = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(tempPassword),
            FullName = request.FullName,
            Role = "Manager",
            VenueId = venue.Id,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _userRepo.AddAsync(manager, ct);
        await _userRepo.SaveChangesAsync(ct);

        venue.PrimaryManagerId = manager.Id;
        _venueRepo.Update(venue);
        await _venueRepo.SaveChangesAsync(ct);

        await _emailService.SendAsync(
            request.Email,
            "Tai khoan BidaPlatform",
            $"""
            Xin chao {request.FullName},

            Tai khoan Manager va quan "{request.VenueName}" da duoc tao thanh cong.

            Thong tin dang nhap:
            - Email: {request.Email}
            - Mat khau tam thoi: {tempPassword}

            Vui long dang nhap va doi mat khau ngay.

            --
            BidaPlatform System
            """
        );
    }

    private static string GenerateRandomPassword(int length)
    {
        const string chars =
            "ABCDEFGHJKLMNPQRSTUVWXYZ" +
            "abcdefghijkmnopqrstuvwxyz" +
            "23456789!@#$%";

        var bytes = RandomNumberGenerator.GetBytes(length);
        var result = new char[length];

        for (int i = 0; i < length; i++)
            result[i] = chars[bytes[i] % chars.Length];

        return new string(result);
    }
}
