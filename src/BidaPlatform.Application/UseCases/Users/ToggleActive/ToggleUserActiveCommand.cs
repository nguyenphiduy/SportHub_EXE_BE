using MediatR;

namespace BidaPlatform.Application.UseCases.Users.ToggleActive;

public class ToggleUserActiveCommand : IRequest
{
    public Guid AdminId { get; }
    public Guid TargetUserId { get; }
    public bool IsActive { get; }
    public string RequesterRole { get; }

    public ToggleUserActiveCommand(
        Guid adminId,
        Guid targetUserId,
        bool isActive,
        string requesterRole)
    {
        AdminId = adminId;
        TargetUserId = targetUserId;
        IsActive = isActive;
        RequesterRole = requesterRole;
    }
}
