using MediatR;
using BidaPlatform.Application.Interfaces;
using BidaPlatform.Application.Models.Venues.Common;
using BidaPlatform.Application.Models.Venues.UpdateVenue;
using BidaPlatform.Domain.Interfaces;

namespace BidaPlatform.Application.UseCases.Venues.UpdateVenue;

public record UpdateVenueCommand(Guid ActorUserId, string ActorRole, Guid? ActorVenueId, Guid VenueId, UpdateVenueRequest Request) : IRequest<VenueResponse>;

public class UpdateVenueHandler : IRequestHandler<UpdateVenueCommand, VenueResponse>
{
    private readonly IVenueRepository _venueRepository;
    private readonly IVenueAccessChecker _venueAccessChecker;

    public UpdateVenueHandler(IVenueRepository venueRepository, IVenueAccessChecker venueAccessChecker)
    {
        _venueRepository = venueRepository;
        _venueAccessChecker = venueAccessChecker;
    }

    public async Task<VenueResponse> Handle(UpdateVenueCommand request, CancellationToken ct)
    {
        await _venueAccessChecker.EnsureCanManageVenueAsync(request.ActorUserId, request.ActorRole, request.ActorVenueId, request.VenueId, ct);

        var venue = await _venueRepository.GetByIdWithManagerAsync(request.VenueId, ct)
            ?? throw new KeyNotFoundException("Không tìm thấy quán");

        if (!string.IsNullOrWhiteSpace(request.Request.Name)) venue.Name = request.Request.Name;
        if (!string.IsNullOrWhiteSpace(request.Request.Address)) venue.Address = request.Request.Address;
        if (!string.IsNullOrWhiteSpace(request.Request.Phone)) venue.Phone = request.Request.Phone;
        if (!string.IsNullOrWhiteSpace(request.Request.OwnerName)) venue.OwnerName = request.Request.OwnerName;
        venue.UpdatedAt = DateTime.UtcNow;

        _venueRepository.Update(venue);
        await _venueRepository.SaveChangesAsync(ct);

        return new VenueResponse
        {
            Id = venue.Id,
            Name = venue.Name,
            Address = venue.Address,
            Phone = venue.Phone,
            OwnerName = venue.OwnerName,
            Status = venue.Status.ToString(),
            IsActive = venue.IsActive,
            PrimaryManagerId = venue.PrimaryManagerId,
            PrimaryManagerName = venue.PrimaryManager?.FullName,
            CreatedAt = venue.CreatedAt,
            UpdatedAt = venue.UpdatedAt
        };
    }
}
