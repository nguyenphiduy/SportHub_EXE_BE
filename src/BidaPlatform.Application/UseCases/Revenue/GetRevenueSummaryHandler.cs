using MediatR;
using BidaPlatform.Application.Models.Revenue;
using BidaPlatform.Application.Interfaces;
using BidaPlatform.Domain.Interfaces;

namespace BidaPlatform.Application.UseCases.Revenue;

public class GetRevenueSummaryHandler
    : IRequestHandler<GetRevenueSummaryQuery, RevenueSummaryResponse>
{
    private readonly IRevenueService _revenueService;
    private readonly IUserRepository _userRepo;
    private readonly IVenueAccessChecker _venueAccessChecker;

    public GetRevenueSummaryHandler(
        IRevenueService revenueService,
        IUserRepository userRepo,
        IVenueAccessChecker venueAccessChecker)
    {
        _revenueService = revenueService;
        _userRepo = userRepo;
        _venueAccessChecker = venueAccessChecker;
    }

    public async Task<RevenueSummaryResponse> Handle(
        GetRevenueSummaryQuery request,
        CancellationToken ct)
    {
        var anchor = request.AnchorDate
            ?? DateTime.UtcNow.AddHours(7);

        if (request.VenueId.HasValue)
        {
            var actor = await _userRepo.GetByIdAsync(request.ActorUserId, ct);

            await _venueAccessChecker.EnsureCanAccessVenueAsync(
                request.ActorUserId,
                actor?.Role ?? string.Empty,
                actor?.VenueId,
                request.VenueId.Value,
                ct);
        }

        return await _revenueService.GetSummaryAsync(
            request.VenueId,
            request.Period.ToLower(),
            anchor,
            ct);
    }
}
