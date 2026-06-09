using MediatR;
using BidaPlatform.Application.Interfaces;
using BidaPlatform.Domain.Interfaces;
using BidaPlatform.Application.Models.Shifts.Common;

namespace BidaPlatform.Application.UseCases.Shifts.DeleteShift;

public record DeleteShiftCommand(Guid ActorUserId, string ActorRole, Guid ShiftId) : IRequest<bool>;

public class DeleteShiftHandler : IRequestHandler<DeleteShiftCommand, bool>
{
    private readonly IWorkShiftRepository _workShiftRepository;
    private readonly IUserRepository _userRepository;
    private readonly INotificationBroadcaster _notificationBroadcaster;

    public DeleteShiftHandler(
        IWorkShiftRepository workShiftRepository,
        IUserRepository userRepository,
        INotificationBroadcaster notificationBroadcaster)
    {
        _workShiftRepository = workShiftRepository;
        _userRepository = userRepository;
        _notificationBroadcaster = notificationBroadcaster;
    }

    public async Task<bool> Handle(DeleteShiftCommand request, CancellationToken ct)
    {
        if (request.ActorRole is not ("SuperAdmin" or "Manager"))
            throw new UnauthorizedAccessException("Chỉ SuperAdmin hoặc Manager mới được xóa ca");

        var actor = await _userRepository.GetByIdAsync(request.ActorUserId, ct)
            ?? throw new UnauthorizedAccessException("Không tìm thấy người dùng");

        var shift = await _workShiftRepository.GetByIdAsync(request.ShiftId, ct)
            ?? throw new ArgumentException("Không tìm thấy ca làm");

        await _workShiftRepository.DeleteAsync(request.ShiftId, ct);
        await _workShiftRepository.SaveChangesAsync(ct);
        await _notificationBroadcaster.BroadcastAsync("shift", "deleted", shift.VenueId);

        return true;
    }
}
