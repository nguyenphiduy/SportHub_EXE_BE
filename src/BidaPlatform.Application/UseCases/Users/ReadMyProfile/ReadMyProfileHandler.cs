using MediatR;
using BidaPlatform.Application.Models.Users.ReadMyProfile;
using BidaPlatform.Domain.Interfaces;

namespace BidaPlatform.Application.UseCases.Users.ReadMyProfile;

public class ReadMyProfileHandler
    : IRequestHandler<ReadMyProfileQuery, ReadMyProfileResponse>
{
    private readonly IUserRepository _userRepo;

    public ReadMyProfileHandler(IUserRepository userRepo)
    {
        _userRepo = userRepo;
    }

    public async Task<ReadMyProfileResponse> Handle(
        ReadMyProfileQuery request,
        CancellationToken ct)
    {
        var user = await _userRepo.GetByIdAsync(request.UserId, ct);

        if (user == null)
            throw new KeyNotFoundException("Không tìm thấy người dùng");

        if (!user.IsActive)
            throw new UnauthorizedAccessException("Tài khoản đã bị vô hiệu hóa");

        return new ReadMyProfileResponse
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role,
            VenueId = user.VenueId,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };
    }
}
