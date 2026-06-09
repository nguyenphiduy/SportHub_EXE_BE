using MediatR;
using BidaPlatform.Application.Models.Venues.Common;

namespace BidaPlatform.Application.UseCases.Venues.ApproveVenue;

public record ApproveVenueCommand(Guid VenueId) : IRequest<VenueResponse>;
