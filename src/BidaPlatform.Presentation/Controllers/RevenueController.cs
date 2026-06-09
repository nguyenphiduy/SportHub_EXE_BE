using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BidaPlatform.Application.UseCases.Revenue;
using System.Security.Claims;

namespace BidaPlatform.Presentation.Controllers;

[ApiController]
[Route("api/revenue")]
[Authorize(Roles = "SuperAdmin,Manager,Staff")]
public class RevenueController : ControllerBase
{
    private readonly IMediator _mediator;

    public RevenueController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetRevenueSummary(
        [FromQuery] Guid? venueId,
        [FromQuery] string period = "week",
        [FromQuery] DateTime? date = null)
    {
        if (period is not ("day" or "week" or "month" or "year"))
            return BadRequest(new { message = "period phải là 'day', 'week', 'month' hoặc 'year'." });

        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _mediator.Send(new GetRevenueSummaryQuery(userId, venueId, period, date));
        return Ok(result);
    }
}
