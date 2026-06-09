using MediatR;
using BidaPlatform.Application.Models.Venues.Common;
using BidaPlatform.Domain.Enums;
using BidaPlatform.Domain.Interfaces;

namespace BidaPlatform.Application.UseCases.Venues.ApproveVenue;

public class ApproveVenueHandler : IRequestHandler<ApproveVenueCommand, VenueResponse>
{
    private readonly IVenueRepository _venueRepository;

    public ApproveVenueHandler(IVenueRepository venueRepository)
    {
        _venueRepository = venueRepository;
    }

    public async Task<VenueResponse> Handle(ApproveVenueCommand request, CancellationToken ct)
    {
        var venue = await _venueRepository.GetByIdWithManagerAsync(request.VenueId, ct)
            ?? throw new KeyNotFoundException("Không tìm thấy quán");

        if (venue.Status != VenueStatus.Pending)
            throw new InvalidOperationException("Chỉ có thể phê duyệt quán đang chờ duyệt");

        venue.Status = VenueStatus.Approved;
        venue.IsActive = true;
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
