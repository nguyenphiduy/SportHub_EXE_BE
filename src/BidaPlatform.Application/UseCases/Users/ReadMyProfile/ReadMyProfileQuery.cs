using MediatR;
using BidaPlatform.Application.Models.Users.ReadMyProfile;

namespace BidaPlatform.Application.UseCases.Users.ReadMyProfile;

public class ReadMyProfileQuery : IRequest<ReadMyProfileResponse>
{
    public Guid UserId { get; }

    public ReadMyProfileQuery(Guid userId)
    {
        UserId = userId;
    }
}
