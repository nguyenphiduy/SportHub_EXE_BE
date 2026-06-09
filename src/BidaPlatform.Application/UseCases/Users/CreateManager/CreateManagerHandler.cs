using MediatR;
using System.Security.Cryptography;
using BidaPlatform.Domain.Entities;
using BidaPlatform.Domain.Enums;
using BidaPlatform.Domain.Interfaces;

namespace BidaPlatform.Application.UseCases.Users.CreateManager;

public class CreateManagerHandler : IRequestHandler<CreateManagerCommand>
{
    private readonly IUserRepository _userRepo;
    private readonly IEmailService _emailService;

    public CreateManagerHandler(IUserRepository userRepo, IEmailService emailService)
    {
        _userRepo = userRepo;
        _emailService = emailService;
    }

    public async Task Handle(CreateManagerCommand request, CancellationToken ct)
    {
        var exists = await _userRepo.GetByEmailWithoutDecryptAsync(request.Email, ct);

        if (exists != null)
            throw new InvalidOperationException("Email đã tồn tại");

        var rawPassword = GeneratePassword(12);
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(rawPassword);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Password = hashedPassword,
            FullName = request.FullName,
            Role = UserRole.Manager.ToString(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _userRepo.AddAsync(user, ct);
        await _userRepo.SaveChangesAsync(ct);

        await _emailService.SendAsync(
            request.Email,
            "Tài khoản BidaPlatform của bạn",
            $"""
            Xin chào {request.FullName},

            Tài khoản Manager của bạn đã được tạo thành công.

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
