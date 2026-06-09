using MediatR;
using BidaPlatform.Domain.Interfaces;

namespace BidaPlatform.Application.UseCases.Auth.ResetPassword;

public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand>
{
    private readonly IUserRepository _userRepo;
    private readonly IAuthTokenRepository _tokenRepo;

    public ResetPasswordHandler(
        IUserRepository userRepo,
        IAuthTokenRepository tokenRepo)
    {
        _userRepo = userRepo;
        _tokenRepo = tokenRepo;
    }

    public async Task Handle(
    ResetPasswordCommand request,
    CancellationToken ct)
    {
        var tokenEntity =
            await _tokenRepo.GetRefreshTokenAsync(request.Token, ct);

        if (tokenEntity == null ||
            tokenEntity.RefreshTokenExpiresAt < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Reset token không hợp lệ");

        var user = tokenEntity.User
            ?? throw new UnauthorizedAccessException();

        // 🔐 Update password
        user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        _userRepo.UpdatePasswordOnly(user);

        // 🔥 Revoke reset token
        tokenEntity.IsRevoked = true;
        _tokenRepo.Update(tokenEntity);

        // 🔥 Revoke all sessions
        foreach (var t in user.AuthTokens)
        {
            t.IsRevoked = true;
            _tokenRepo.Update(t);
        }

        await _tokenRepo.SaveChangeAsync(ct);
    }

}
