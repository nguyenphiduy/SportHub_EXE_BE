using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BidaPlatform.Application.UseCases.Dashboard.GetDashboardSummary;
using BidaPlatform.Application.Interfaces;

namespace BidaPlatform.Presentation.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize(Roles = "SuperAdmin,Manager,Staff")]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserContext _currentUser;

    public DashboardController(IMediator mediator, ICurrentUserContext currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> GetSummary([FromQuery] Guid? venueId)
    {
        var effectiveVenueId = _currentUser.Role == "SuperAdmin" ? venueId : _currentUser.VenueId;
        var result = await _mediator.Send(new GetDashboardSummaryQuery(_currentUser.UserId, _currentUser.Role, effectiveVenueId));
        return Ok(result);
    }
}
