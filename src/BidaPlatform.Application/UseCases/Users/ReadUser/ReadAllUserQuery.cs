using MediatR;
using BidaPlatform.Application.Models.Users.ReadAllUser;

namespace BidaPlatform.Application.UseCases.Users.ReadAllUser;

public class ReadAllUserQuery : IRequest<List<ReadAllUserResponse>>
{
    public Guid RequesterId { get; }
    public string RequesterRole { get; }
    public string? RoleFilter { get; }

    public ReadAllUserQuery(Guid requesterId, string requesterRole, string? roleFilter = null)
    {
        RequesterId = requesterId;
        RequesterRole = requesterRole;
        RoleFilter = roleFilter;
    }
}
