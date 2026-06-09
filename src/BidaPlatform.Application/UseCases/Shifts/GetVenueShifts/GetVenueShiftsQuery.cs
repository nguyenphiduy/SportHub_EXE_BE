using MediatR;
using BidaPlatform.Application.Interfaces;
using BidaPlatform.Application.Models.Shifts.Common;
using BidaPlatform.Domain.Interfaces;

namespace BidaPlatform.Application.UseCases.Shifts.GetVenueShifts;

public record GetVenueShiftsQuery(Guid ActorUserId, string ActorRole, Guid VenueId) : IRequest<List<ShiftResponse>>;

public class GetVenueShiftsHandler : IRequestHandler<GetVenueShiftsQuery, List<ShiftResponse>>
{
    private readonly IWorkShiftRepository _workShiftRepository;
    private readonly IUserRepository _userRepository;
    private readonly IVenueAccessChecker _venueAccessChecker;

    public GetVenueShiftsHandler(
        IWorkShiftRepository workShiftRepository,
        IUserRepository userRepository,
        IVenueAccessChecker venueAccessChecker)
    {
        _workShiftRepository = workShiftRepository;
        _userRepository = userRepository;
        _venueAccessChecker = venueAccessChecker;
    }

    public async Task<List<ShiftResponse>> Handle(GetVenueShiftsQuery request, CancellationToken ct)
    {
        var actor = await _userRepository.GetByIdAsync(request.ActorUserId, ct);

        await _venueAccessChecker.EnsureCanAccessVenueAsync(
            request.ActorUserId,
            actor?.Role ?? string.Empty,
            actor?.VenueId,
            request.VenueId,
            ct);

        var shifts = await _workShiftRepository.GetByVenueIdAsync(request.VenueId, ct);

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
