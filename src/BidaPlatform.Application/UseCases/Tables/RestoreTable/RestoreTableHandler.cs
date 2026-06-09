using MediatR;
using BidaPlatform.Domain.Enums;
using BidaPlatform.Domain.Interfaces;
using BidaPlatform.Application.Interfaces;

namespace BidaPlatform.Application.UseCases.Tables.RestoreTable;

public class RestoreTableHandler : IRequestHandler<RestoreTableCommand>
{
    private readonly IBilliardTableRepository _tableRepo;
    private readonly IVenueAccessChecker _venueAccessChecker;

    public RestoreTableHandler(IBilliardTableRepository tableRepo, IVenueAccessChecker venueAccessChecker)
    {
        _tableRepo = tableRepo;
        _venueAccessChecker = venueAccessChecker;
    }

    public async Task Handle(RestoreTableCommand request, CancellationToken ct)
    {
        await _venueAccessChecker.EnsureCanManageVenueAsync(request.ActorUserId, UserRole.Manager.ToString(), request.VenueId, request.VenueId, ct);

        var table = await _tableRepo.GetByIdAsync(request.TableId, ct)
            ?? throw new KeyNotFoundException($"Không tìm thấy bàn {request.TableId}");

        if (table.VenueId != request.VenueId)
            throw new UnauthorizedAccessException("Không thể khôi phục bàn của quán khác");

        table.IsActive = true;
        table.UpdatedAt = DateTime.UtcNow;
        _tableRepo.Update(table);
        await _tableRepo.SaveChangesAsync(ct);
    }
}
