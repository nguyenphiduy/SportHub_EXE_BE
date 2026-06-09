using MediatR;
using BidaPlatform.Domain.Interfaces;
using BidaPlatform.Domain.Enums;

namespace BidaPlatform.Application.UseCases.Users.UpdateUser;

public class UpdateProfileHandler
    : IRequestHandler<UpdateProfileCommand>
{
    private readonly IUserRepository _userRepo;

    public UpdateProfileHandler(IUserRepository userRepo)
    {
        _userRepo = userRepo;
    }

    public async Task Handle(
        UpdateProfileCommand request,
        CancellationToken ct)
    {
        if (request.Role == UserRole.SuperAdmin.ToString())
            throw new UnauthorizedAccessException(
                "SuperAdmin không được phép sửa thông tin cá nhân ở endpoint này");

        var user = await _userRepo
            .GetByIdWithoutDecryptAsync(request.UserId, ct)
            ?? throw new UnauthorizedAccessException("User không tồn tại");

        if (!user.IsActive)
            throw new UnauthorizedAccessException("Tài khoản đã bị khóa");

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var exists = await _userRepo
                .GetByEmailWithoutDecryptAsync(request.Email, ct);

            if (exists != null && exists.Id != user.Id)
                throw new InvalidOperationException("Email đã tồn tại");

            user.Email = request.Email;
        }

        if (!string.IsNullOrWhiteSpace(request.FullName))
        {
            user.FullName = request.FullName;
        }

        user.UpdatedAt = DateTime.UtcNow;

        _userRepo.Update(user);
        await _userRepo.SaveChangesAsync(ct);
    }
}
