using MediatR;
using BidaPlatform.Application.Interfaces;
using BidaPlatform.Application.Models.Shifts.Common;
using BidaPlatform.Application.Models.Shifts.CreateShift;
using BidaPlatform.Domain.Entities;
using BidaPlatform.Domain.Interfaces;

namespace BidaPlatform.Application.UseCases.Shifts.CreateShift;

public record CreateShiftCommand(Guid ActorUserId, string ActorRole, CreateShiftRequest Request) : IRequest<ShiftResponse>;

public class CreateShiftHandler : IRequestHandler<CreateShiftCommand, ShiftResponse>
{
    private readonly IWorkShiftRepository _workShiftRepository;
    private readonly IUserRepository _userRepository;
    private readonly IVenueAccessChecker _venueAccessChecker;
    private readonly INotificationBroadcaster _notificationBroadcaster;

    public CreateShiftHandler(
        IWorkShiftRepository workShiftRepository,
        IUserRepository userRepository,
        IVenueAccessChecker venueAccessChecker,
        INotificationBroadcaster notificationBroadcaster)
    {
        _workShiftRepository = workShiftRepository;
        _userRepository = userRepository;
        _venueAccessChecker = venueAccessChecker;
        _notificationBroadcaster = notificationBroadcaster;
    }

    public async Task<ShiftResponse> Handle(CreateShiftCommand request, CancellationToken ct)
    {
        if (request.ActorRole is not ("SuperAdmin" or "Manager"))
            throw new UnauthorizedAccessException("Chỉ SuperAdmin hoặc Manager mới được tạo ca");

        var actor = await _userRepository.GetByIdAsync(request.ActorUserId, ct)
            ?? throw new UnauthorizedAccessException("Không tìm thấy người dùng");

        if (request.Request is null)
            throw new ArgumentException("Request không hợp lệ");

        if (!DateTime.TryParse(request.Request.ShiftDate, out var shiftDate))
            throw new ArgumentException("ShiftDate không hợp lệ (định dạng: yyyy-MM-dd)");

        if (!TimeSpan.TryParse(request.Request.StartTime, out var startTime))
            throw new ArgumentException("StartTime không hợp lệ (định dạng: HH:mm)");

        if (!TimeSpan.TryParse(request.Request.EndTime, out var endTime))
            throw new ArgumentException("EndTime không hợp lệ (định dạng: HH:mm)");

        var venueId = request.Request.VenueId ?? actor.VenueId
            ?? throw new ArgumentException("VenueId không hợp lệ");

        await _venueAccessChecker.EnsureCanManageVenueAsync(
            request.ActorUserId,
            request.ActorRole,
            actor.VenueId,
            venueId,
            ct);

        var shift = new WorkShift
        {
            Id = Guid.NewGuid(),
            VenueId = venueId,
            StaffUserId = request.Request.StaffUserId,
            Name = request.Request.Name,
            ShiftDate = DateTime.SpecifyKind(shiftDate.Date, DateTimeKind.Utc),
            StartTime = DateTime.SpecifyKind(DateTime.Today.Add(startTime), DateTimeKind.Utc),
            EndTime = DateTime.SpecifyKind(DateTime.Today.Add(endTime), DateTimeKind.Utc),
            IsCheckedIn = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _workShiftRepository.AddAsync(shift, ct);
        await _workShiftRepository.SaveChangesAsync(ct);
        await _notificationBroadcaster.BroadcastAsync("shift", "created", shift.VenueId);

        return await Map(shift, ct);
    }

    private async Task<ShiftResponse> Map(WorkShift shift, CancellationToken ct)
    {
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
