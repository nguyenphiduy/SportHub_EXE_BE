using MediatR;
using BidaPlatform.Application.Models.Tables.ViewTables;
using BidaPlatform.Application.Interfaces;
using BidaPlatform.Domain.Interfaces;

namespace BidaPlatform.Application.UseCases.Tables.ViewTables;

public class ReadAllTablesHandler : IRequestHandler<ReadAllTablesQuery, IEnumerable<TableResponse>>
{
    private readonly IBilliardTableRepository _tableRepo;
    private readonly IUserRepository _userRepo;
    private readonly IVenueAccessChecker _venueAccessChecker;

    public ReadAllTablesHandler(
        IBilliardTableRepository tableRepo,
        IUserRepository userRepo,
        IVenueAccessChecker venueAccessChecker)
    {
        _tableRepo = tableRepo;
        _userRepo = userRepo;
        _venueAccessChecker = venueAccessChecker;
    }

    public async Task<IEnumerable<TableResponse>> Handle(ReadAllTablesQuery request, CancellationToken ct)
    {
        var actor = await _userRepo.GetByIdAsync(request.ActorUserId, ct)
            ?? throw new UnauthorizedAccessException("Không tìm thấy người dùng");

        await _venueAccessChecker.EnsureCanAccessVenueAsync(
            request.ActorUserId,
            actor.Role,
            actor.VenueId,
            request.VenueId,
            ct);

        var tables = request.ActiveOnly
            ? await _tableRepo.GetActiveByVenueAsync(request.VenueId, ct)
            : await _tableRepo.GetAllByVenueAsync(request.VenueId, ct);

        return tables.Select(t => new TableResponse
        {
            Id = t.Id,
            Name = t.Name,
            Type = t.Type.ToString(),
            PricePerHour = t.PricePerHour,
            Status = t.Status.ToString(),
            IsActive = t.IsActive,
            Device = t.IoTDevice == null ? null : new IoTDeviceInfo
            {
                Id = t.IoTDevice.Id,
                IpAddress = t.IoTDevice.IpAddress,
                DeviceName = t.IoTDevice.DeviceName,
                IsOnline = t.IoTDevice.IsOnline
            }
        });
    }
}
