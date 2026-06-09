using MediatR;
using BidaPlatform.Domain.Interfaces;

namespace BidaPlatform.Application.UseCases.Auth.ChangePassword;

public class ChangePasswordHandler
    : IRequestHandler<ChangePasswordCommand>
{
    private readonly IUserRepository _userRepo;
    private readonly IAuthTokenRepository _tokenRepo;

    public ChangePasswordHandler(
        IUserRepository userRepo,
        IAuthTokenRepository tokenRepo)
    {
        _userRepo = userRepo;
        _tokenRepo = tokenRepo;
    }

    public async Task Handle(
        ChangePasswordCommand request,
        CancellationToken ct)
    {
        // 🔥 BUSINESS LOGIC ONLY
        var user = await _userRepo.GetByIdAsync(request.UserId, ct)
            ?? throw new UnauthorizedAccessException();

        if (!BCrypt.Net.BCrypt.Verify(
            request.CurrentPassword, user.Password))
            throw new UnauthorizedAccessException("Mật khẩu hiện tại không đúng");

        user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        _userRepo.UpdatePasswordOnly(user);

        // 🔥 revoke all tokens
        foreach (var token in user.AuthTokens)
        {
            token.IsRevoked = true;
            _tokenRepo.Update(token);
        }

        await _tokenRepo.SaveChangeAsync(ct);
    }
}
