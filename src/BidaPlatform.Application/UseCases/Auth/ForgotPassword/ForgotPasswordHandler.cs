using MediatR;
using System.Security.Cryptography;
using BidaPlatform.Domain.Entities;
using BidaPlatform.Domain.Interfaces;

namespace BidaPlatform.Application.UseCases.Auth.ForgotPassword;

public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand>
{
    private readonly IUserRepository _userRepo;
    private readonly IAuthTokenRepository _tokenRepo;
    private readonly IEmailService _emailService;

    public ForgotPasswordHandler(
        IUserRepository userRepo,
        IAuthTokenRepository tokenRepo,
        IEmailService emailService)
    {
        _userRepo = userRepo;
        _tokenRepo = tokenRepo;
        _emailService = emailService;
    }

    public async Task Handle(
        ForgotPasswordCommand request,
        CancellationToken ct)
    {
        var user = await _userRepo.GetByEmailAsync(request.Email, ct);

        // ❗ Không leak thông tin
        if (user == null || !user.IsActive)
            return;

        // 🔐 Generate secure token (256-bit)
        var resetToken = Convert.ToBase64String(
            RandomNumberGenerator.GetBytes(32));

        var resetEntity = new AuthToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,

            AccessToken = string.Empty,
            RefreshToken = resetToken,

            CreatedAt = DateTime.UtcNow,
            RefreshTokenExpiresAt = DateTime.UtcNow.AddMinutes(15),
            IsRevoked = false
        };

        await _tokenRepo.SaveResetTokenAsync(resetEntity, ct);
        await _tokenRepo.SaveChangeAsync(ct);

        var resetLink =
            $"http://localhost:5173/reset-password?token={Uri.EscapeDataString(resetToken)}";


        await _emailService.SendAsync(
            user.Email,
            "Reset mật khẩu BidaPlatform",
            $"""
            Xin chào,

            Bạn đã yêu cầu reset mật khẩu.

            Vui lòng click link dưới đây để đặt mật khẩu mới
            (link chỉ có hiệu lực trong 15 phút):

            {resetLink}

            Nếu không phải bạn, hãy bỏ qua email này.

            --
            BidaPlatform System
            """
        );
    }
}
