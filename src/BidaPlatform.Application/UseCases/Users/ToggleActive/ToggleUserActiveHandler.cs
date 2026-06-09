using MediatR;
using BidaPlatform.Domain.Interfaces;
using BidaPlatform.Domain.Enums;
using BidaPlatform.Application.Interfaces;

namespace BidaPlatform.Application.UseCases.Users.ToggleActive;

public class ToggleUserActiveHandler
    : IRequestHandler<ToggleUserActiveCommand>
{
    private readonly IUserRepository _userRepo;
    private readonly INotificationBroadcaster _broadcaster;

    public ToggleUserActiveHandler(IUserRepository userRepo, INotificationBroadcaster broadcaster)
    {
        _userRepo = userRepo;
        _broadcaster = broadcaster;
    }

    public async Task Handle(
        ToggleUserActiveCommand request,
        CancellationToken ct)
    {
        var requester = await _userRepo
            .GetByIdWithoutDecryptAsync(request.AdminId, ct)
            ?? throw new UnauthorizedAccessException();

        if (requester.Role == UserRole.SuperAdmin.ToString())
        {
            if (request.AdminId == request.TargetUserId)
                throw new InvalidOperationException(
                    "Không thể thay đổi trạng thái chính mình");

            var target = await _userRepo
                .GetByIdWithoutDecryptAsync(request.TargetUserId, ct)
                ?? throw new InvalidOperationException("User không tồn tại");

            if (target.Role == UserRole.SuperAdmin.ToString())
                throw new InvalidOperationException(
                    "Không thể thay đổi trạng thái SuperAdmin");
        }
        else if (requester.Role == UserRole.Manager.ToString())
        {
            if (requester.VenueId is null)
                throw new UnauthorizedAccessException("Manager chưa được gán quán");

            var target = await _userRepo
                .GetByIdWithoutDecryptAsync(request.TargetUserId, ct)
                ?? throw new InvalidOperationException("User không tồn tại");

            if (target.Role == UserRole.SuperAdmin.ToString())
                throw new InvalidOperationException(
                    "Không thể thay đổi trạng thái SuperAdmin");

            if (target.VenueId != requester.VenueId)
                throw new UnauthorizedAccessException(
                    "Bạn chỉ có thể thay đổi trạng thái staff thuộc quán của mình");

            if (target.Role != UserRole.Staff.ToString())
                throw new InvalidOperationException(
                    "Bạn chỉ có thể thay đổi trạng thái Staff");
        }
        else
        {
            throw new UnauthorizedAccessException("Bạn không có quyền thực hiện thao tác này");
        }

        var targetUser = await _userRepo.GetByIdAsync(request.TargetUserId, ct)
            ?? throw new InvalidOperationException("User không tồn tại");

        targetUser.IsActive = request.IsActive;
        targetUser.UpdatedAt = DateTime.UtcNow;

        _userRepo.UpdateV1(targetUser);
        await _userRepo.SaveChangesAsync(ct);
        await _broadcaster.BroadcastAsync("user", "updated");
    }
}
