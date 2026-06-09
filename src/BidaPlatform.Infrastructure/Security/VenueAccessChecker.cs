using BidaPlatform.Application.Interfaces;
using BidaPlatform.Domain.Enums;
using BidaPlatform.Domain.Interfaces;

namespace BidaPlatform.Infrastructure.Security;

public class VenueAccessChecker : IVenueAccessChecker
{
    private readonly IUserRepository _userRepository;
    private readonly IVenueRepository _venueRepository;

    public VenueAccessChecker(IUserRepository userRepository, IVenueRepository venueRepository)
    {
        _userRepository = userRepository;
        _venueRepository = venueRepository;
    }

    public async Task EnsureCanAccessVenueAsync(Guid actorUserId, string actorRole, Guid? actorVenueId, Guid targetVenueId, CancellationToken ct = default)
    {
        if (actorRole == UserRole.SuperAdmin.ToString())
            return;

        if (actorRole is not (nameof(UserRole.Manager) or nameof(UserRole.Staff)))
            throw new UnauthorizedAccessException("Role không được phép truy cập quán này");

        var user = await _userRepository.GetByIdWithoutDecryptAsync(actorUserId, ct)
            ?? throw new UnauthorizedAccessException("Không tìm thấy người dùng");

        if (!user.IsActive)
            throw new UnauthorizedAccessException("Tài khoản đã bị khóa");

        if (actorVenueId != targetVenueId)
            throw new UnauthorizedAccessException("Bạn không thể truy cập dữ liệu của quán khác");

        await EnsureVenueApprovedAsync(targetVenueId, ct);
    }

    public async Task EnsureCanManageVenueAsync(Guid actorUserId, string actorRole, Guid? actorVenueId, Guid targetVenueId, CancellationToken ct = default)
    {
        if (actorRole == UserRole.SuperAdmin.ToString())
            return;

        if (actorRole != UserRole.Manager.ToString())
            throw new UnauthorizedAccessException("Chỉ Manager hoặc SuperAdmin mới được quản lý quán");

        await EnsureCanAccessVenueAsync(actorUserId, actorRole, actorVenueId, targetVenueId, ct);
    }

    public async Task EnsureVenueApprovedAsync(Guid venueId, CancellationToken ct = default)
    {
        var venue = await _venueRepository.GetByIdAsync(venueId, ct);
        if (venue == null)
            throw new KeyNotFoundException("Không tìm thấy quán");

        if (venue.Status != VenueStatus.Approved)
            throw new UnauthorizedAccessException("Quán chưa được phê duyệt. Vui lòng chờ Admin duyệt.");
    }
}
