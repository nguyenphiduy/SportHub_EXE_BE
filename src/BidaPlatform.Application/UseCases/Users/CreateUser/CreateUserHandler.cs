using MediatR;
using System.Security.Cryptography;
using BidaPlatform.Domain.Entities;
using BidaPlatform.Domain.Enums;
using BidaPlatform.Domain.Interfaces;
using BidaPlatform.Application.Interfaces;

namespace BidaPlatform.Application.UseCases.Users.CreateUser;

public class CreateUserHandler : IRequestHandler<CreateUserCommand>
{
    private readonly IUserRepository _userRepo;
    private readonly IEmailService _emailService;
    private readonly INotificationBroadcaster _broadcaster;

    public CreateUserHandler(
        IUserRepository userRepo,
        IEmailService emailService,
        INotificationBroadcaster broadcaster)
    {
        _userRepo = userRepo;
        _emailService = emailService;
        _broadcaster = broadcaster;
    }

    public async Task Handle(
        CreateUserCommand request,
        CancellationToken ct)
    {
        var exists = await _userRepo
            .GetByEmailWithoutDecryptAsync(request.Email, ct);

        if (exists != null)
            throw new InvalidOperationException("Email đã tồn tại");

        var creator = await _userRepo
            .GetByIdAsync(request.CreatorUserId, ct)
            ?? throw new UnauthorizedAccessException();

        if (!string.Equals(
                creator.Role,
                UserRole.Manager.ToString(),
                StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedAccessException(
                "Chỉ Manager mới được tạo tài khoản Staff");
        }

        if (creator.VenueId is null)
            throw new InvalidOperationException("Manager chưa được gán quán");

        if (creator.VenueId != request.VenueId)
            throw new UnauthorizedAccessException("Bạn chỉ có thể tạo Staff cho quán của mình");

        var rawPassword = GeneratePassword(12);
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(rawPassword);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Password = hashedPassword,
            FullName = request.FullName,
            Role = UserRole.Staff.ToString(),
            VenueId = request.VenueId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _userRepo.AddAsync(user, ct);
        await _userRepo.SaveChangesAsync(ct);
        await _broadcaster.BroadcastAsync("user", "created");

        await _emailService.SendAsync(
            request.Email,
            "Tài khoản BidaPlatform của bạn",
            $"""
            Xin chào {request.FullName},

            Tài khoản nhân viên của bạn đã được tạo thành công.

            Thông tin đăng nhập:
            - Email: {request.Email}
            - Mật khẩu tạm thời: {rawPassword}

            Vui lòng đăng nhập và đổi mật khẩu ngay.

            --
            BidaPlatform System
            """
        );
    }

    private static string GeneratePassword(int length)
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
