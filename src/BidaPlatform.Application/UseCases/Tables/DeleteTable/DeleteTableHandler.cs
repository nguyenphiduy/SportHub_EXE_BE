using MediatR;
using BidaPlatform.Domain.Enums;
using BidaPlatform.Domain.Interfaces;
using BidaPlatform.Application.Interfaces;

namespace BidaPlatform.Application.UseCases.Tables.DeleteTable;

public class DeleteTableHandler : IRequestHandler<DeleteTableCommand>
{
    private readonly IBilliardTableRepository _tableRepo;
    private readonly IVenueAccessChecker _venueAccessChecker;

    public DeleteTableHandler(IBilliardTableRepository tableRepo, IVenueAccessChecker venueAccessChecker)
    {
        _tableRepo = tableRepo;
        _venueAccessChecker = venueAccessChecker;
    }

    public async Task Handle(DeleteTableCommand request, CancellationToken ct)
    {
        await _venueAccessChecker.EnsureCanManageVenueAsync(request.ActorUserId, UserRole.Manager.ToString(), request.VenueId, request.VenueId, ct);

        var table = await _tableRepo.GetByIdAsync(request.TableId, ct)
            ?? throw new KeyNotFoundException($"Không tìm thấy bàn {request.TableId}");

        if (table.VenueId != request.VenueId)
            throw new UnauthorizedAccessException("Không thể xóa bàn của quán khác");

        if (table.Status == BilliardTableStatus.Playing)
            throw new InvalidOperationException("Không thể xóa bàn đang có session chơi.");

        table.IsActive = false;
        table.UpdatedAt = DateTime.UtcNow;
        _tableRepo.Update(table);
        await _tableRepo.SaveChangesAsync(ct);
    }
}
