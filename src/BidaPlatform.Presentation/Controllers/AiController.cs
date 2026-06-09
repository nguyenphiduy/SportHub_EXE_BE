using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BidaPlatform.Application.UseCases.AI.GetVenueInsights;
using BidaPlatform.Application.UseCases.AI.GetAiAnalysisHistory;
using System.Security.Claims;

namespace BidaPlatform.Presentation.Controllers;

[ApiController]
[Route("api/ai")]
[Authorize(Roles = "SuperAdmin,Manager")]
public class AiController : ControllerBase
{
    private readonly IMediator _mediator;

    public AiController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("venues/{venueId:guid}/insights")]
    public async Task<IActionResult> GetVenueInsights(Guid venueId, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
    {
        var actorUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var actorRole = User.FindFirstValue("role")!;
        var actorVenueIdClaim = User.FindFirstValue("venueId");
        Guid? actorVenueId = string.IsNullOrEmpty(actorVenueIdClaim) ? null : Guid.Parse(actorVenueIdClaim);
        var result = await _mediator.Send(new GetVenueInsightsQuery(actorUserId, actorRole, actorVenueId, venueId, fromDate, toDate));
        return Ok(result);
    }

    [HttpGet("venues/{venueId:guid}/history")]
    public async Task<IActionResult> GetVenueAiHistory(Guid venueId, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
    {
        var actorRole = User.FindFirstValue("role")!;
        if (actorRole is not ("SuperAdmin" or "Manager"))
            return Forbid();

        var actorVenueIdClaim = User.FindFirstValue("venueId");
        Guid? actorVenueId = string.IsNullOrEmpty(actorVenueIdClaim) ? null : Guid.Parse(actorVenueIdClaim);
        var actorUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _mediator.Send(new GetAiAnalysisHistoryQuery(actorUserId, actorRole, actorVenueId, venueId, fromDate, toDate));
        return Ok(result);
    }
}
