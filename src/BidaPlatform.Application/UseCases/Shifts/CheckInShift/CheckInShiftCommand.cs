using MediatR;
using BidaPlatform.Application.Models.Shifts.Common;
using BidaPlatform.Domain.Interfaces;

namespace BidaPlatform.Application.UseCases.Shifts.CheckInShift;

public record CheckInShiftCommand(Guid ActorUserId, string ActorRole, Guid ShiftId) : IRequest<ShiftResponse>;

public class CheckInShiftHandler : IRequestHandler<CheckInShiftCommand, ShiftResponse>
{
    private readonly IWorkShiftRepository _workShiftRepository;

    public CheckInShiftHandler(IWorkShiftRepository workShiftRepository)
    {
        _workShiftRepository = workShiftRepository;
    }

    public async Task<ShiftResponse> Handle(CheckInShiftCommand request, CancellationToken ct)
    {
        var shift = await _workShiftRepository.GetByIdAsync(request.ShiftId, ct)
            ?? throw new KeyNotFoundException("Không tìm thấy ca làm việc");

        if (request.ActorRole == "Staff" && shift.StaffUserId != request.ActorUserId)
            throw new UnauthorizedAccessException("Bạn chỉ có thể chấm công cho ca của mình");

        shift.IsCheckedIn = true;
        shift.CheckedInAt = DateTime.UtcNow;
        shift.UpdatedAt = DateTime.UtcNow;
        _workShiftRepository.Update(shift);
        await _workShiftRepository.SaveChangesAsync(ct);

        return new ShiftResponse
        {
            Id = shift.Id,
            VenueId = shift.VenueId,
            StaffUserId = shift.StaffUserId,
            Name = shift.Name,
            ShiftDate = shift.ShiftDate,
            StartTime = shift.StartTime,
            EndTime = shift.EndTime,
            IsCheckedIn = shift.IsCheckedIn,
            CheckedInAt = shift.CheckedInAt,
            IsActive = shift.IsActive
        };
    }
}
