using MediatR;
using BidaPlatform.Application.Models.Venues.Common;
using BidaPlatform.Domain.Interfaces;

namespace BidaPlatform.Application.UseCases.Venues.ToggleVenueActive;

public record ToggleVenueActiveCommand(Guid VenueId, bool IsActive) : IRequest<VenueResponse>;

public class ToggleVenueActiveHandler : IRequestHandler<ToggleVenueActiveCommand, VenueResponse>
{
    private readonly IVenueRepository _venueRepository;

    public ToggleVenueActiveHandler(IVenueRepository venueRepository)
    {
        _venueRepository = venueRepository;
    }

    public async Task<VenueResponse> Handle(ToggleVenueActiveCommand request, CancellationToken ct)
    {
        var venue = await _venueRepository.GetByIdWithManagerAsync(request.VenueId, ct)
            ?? throw new KeyNotFoundException("Không tìm thấy quán");

        venue.IsActive = request.IsActive;
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
            IsActive = venue.IsActive,
            PrimaryManagerId = venue.PrimaryManagerId,
            PrimaryManagerName = venue.PrimaryManager?.FullName,
            CreatedAt = venue.CreatedAt,
            UpdatedAt = venue.UpdatedAt
        };
    }
}
