using MediatR;
using BidaPlatform.Application.Interfaces;
using BidaPlatform.Application.Models.Shifts.Common;
using BidaPlatform.Domain.Interfaces;

namespace BidaPlatform.Application.UseCases.Shifts.GetShiftById;

public record GetShiftByIdQuery(Guid ShiftId) : IRequest<ShiftResponse>;

public class GetShiftByIdHandler : IRequestHandler<GetShiftByIdQuery, ShiftResponse>
{
    private readonly IWorkShiftRepository _workShiftRepository;
    private readonly IUserRepository _userRepository;

    public GetShiftByIdHandler(
        IWorkShiftRepository workShiftRepository,
        IUserRepository userRepository)
    {
        _workShiftRepository = workShiftRepository;
        _userRepository = userRepository;
    }

    public async Task<ShiftResponse> Handle(GetShiftByIdQuery request, CancellationToken ct)
    {
        var shift = await _workShiftRepository.GetByIdAsync(request.ShiftId, ct)
            ?? throw new ArgumentException("Không tìm thấy ca làm");

        string? staffUserName = null;
        if (shift.StaffUserId.HasValue)
        {
            var staff = await _userRepository.GetByIdAsync(shift.StaffUserId.Value, ct);
            staffUserName = staff?.FullName;
        }

        return new ShiftResponse
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
        };
    }
}
