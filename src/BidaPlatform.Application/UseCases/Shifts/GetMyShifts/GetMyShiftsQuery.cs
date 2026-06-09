using MediatR;
using BidaPlatform.Application.Interfaces;
using BidaPlatform.Application.Models.Shifts.Common;
using BidaPlatform.Domain.Interfaces;

namespace BidaPlatform.Application.UseCases.Shifts.GetMyShifts;

public record GetMyShiftsQuery(Guid UserId) : IRequest<List<ShiftResponse>>;

public class GetMyShiftsHandler : IRequestHandler<GetMyShiftsQuery, List<ShiftResponse>>
{
    private readonly IWorkShiftRepository _workShiftRepository;
    private readonly IUserRepository _userRepository;

    public GetMyShiftsHandler(
        IWorkShiftRepository workShiftRepository,
        IUserRepository userRepository)
    {
        _workShiftRepository = workShiftRepository;
        _userRepository = userRepository;
    }

    public async Task<List<ShiftResponse>> Handle(GetMyShiftsQuery request, CancellationToken ct)
    {
        var shifts = await _workShiftRepository.GetByStaffUserIdAsync(request.UserId, ct);

        var result = new List<ShiftResponse>();
        foreach (var shift in shifts)
        {
            string? staffUserName = null;
            if (shift.StaffUserId.HasValue)
            {
                var staff = await _userRepository.GetByIdAsync(shift.StaffUserId.Value, ct);
                staffUserName = staff?.FullName;
            }

            result.Add(new ShiftResponse
            {
                Id = shift.Id,
                VenueId = shift.VenueId,
                StaffUserId = shift.StaffUserId,
                StaffUserName = staffUserName,
                Name = shift.Name,
                ShiftDate = shift.ShiftDate,
                StartTime = shift.StartTime,
                EndTime = shift.EndTime,
                IsCheckedIn = shift.IsCheckedIn,
                CheckedInAt = shift.CheckedInAt,
                IsActive = shift.IsActive
            });
        }

        return result;
    }
}
