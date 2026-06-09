using MediatR;
using BidaPlatform.Application.Models.Users.ReadAllUser;
using BidaPlatform.Domain.Enums;
using BidaPlatform.Domain.Interfaces;

namespace BidaPlatform.Application.UseCases.Users.ReadAllUser;

public class ReadAllUserHandler
    : IRequestHandler<ReadAllUserQuery, List<ReadAllUserResponse>>
{
    private readonly IUserRepository _userRepo;

    public ReadAllUserHandler(IUserRepository userRepo)
    {
        _userRepo = userRepo;
    }

    public async Task<List<ReadAllUserResponse>> Handle(
        ReadAllUserQuery request,
        CancellationToken ct)
    {
        IEnumerable<Domain.Entities.User> users;

        if (request.RequesterRole == UserRole.SuperAdmin.ToString())
        {
            users = await _userRepo.GetListUserWithVenue();
        }
        else if (request.RequesterRole == UserRole.Manager.ToString())
        {
            var manager = await _userRepo.GetByIdAsync(request.RequesterId, ct)
                ?? throw new UnauthorizedAccessException("Không tìm thấy manager");

            if (manager.VenueId is null)
                throw new UnauthorizedAccessException("Manager chưa được gán quán");

            users = await _userRepo.GetUsersByVenueAsync(manager.VenueId.Value, ct);
        }
        else
        {
            throw new UnauthorizedAccessException(
                "Bạn không có quyền xem danh sách người dùng");
        }

        var result = users.Select(u => new ReadAllUserResponse
        {
            Id = u.Id,
            Email = u.Email,
            FullName = u.FullName,
            Role = u.Role,
            VenueId = u.VenueId,
            VenueName = u.Venue?.Name,
            IsActive = u.IsActive,
            CreatedAt = u.CreatedAt
        }).ToList();

        if (!string.IsNullOrEmpty(request.RoleFilter))
        {
            result = result.Where(u => u.Role == request.RoleFilter).ToList();
        }

        return result;
    }
}
