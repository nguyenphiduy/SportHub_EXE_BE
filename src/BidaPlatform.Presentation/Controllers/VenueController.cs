using BidaPlatform.Application.Models.Venues.RegisterVenue;
using BidaPlatform.Application.UseCases.Venues.GetAllVenues;
using BidaPlatform.Application.UseCases.Venues.GetVenueDetail;
using BidaPlatform.Application.UseCases.Venues.RegisterVenue;
using BidaPlatform.Application.UseCases.Venues.ToggleVenueActive;
using BidaPlatform.Application.UseCases.Venues.UpdateVenue;
using BidaPlatform.Application.UseCases.Venues.ApproveVenue;
using BidaPlatform.Application.UseCases.Venues.RejectVenue;
using BidaPlatform.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BidaPlatform.Presentation.Controllers;

[ApiController]
[Route("api/venues")]
public class VenueController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserContext _currentUser;

    public VenueController(IMediator mediator, ICurrentUserContext currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterVenueRequest request)
    {
        var result = await _mediator.Send(new RegisterVenueCommand(request));
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [Authorize(Roles = "SuperAdmin")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllVenuesQuery());
        return Ok(result);
    }

    [Authorize(Roles = "SuperAdmin,Manager")]
    [HttpGet("{venueId:guid}")]
    public async Task<IActionResult> GetDetail(Guid venueId)
    {
        var targetVenueId = _currentUser.Role == "Manager" ? _currentUser.VenueId ?? venueId : venueId;
        var result = await _mediator.Send(new GetVenueDetailQuery(targetVenueId));
        return Ok(result);
    }

    [Authorize(Roles = "SuperAdmin")]
    [HttpPatch("{venueId:guid}/active")]
    public async Task<IActionResult> ToggleActive(Guid venueId, [FromQuery] bool isActive)
    {
        var result = await _mediator.Send(new ToggleVenueActiveCommand(venueId, isActive));
        return Ok(result);
    }

    [Authorize(Roles = "SuperAdmin")]
    [HttpPatch("{venueId:guid}/approve")]
    public async Task<IActionResult> Approve(Guid venueId)
    {
        var result = await _mediator.Send(new ApproveVenueCommand(venueId));
        return Ok(result);
    }

    [Authorize(Roles = "SuperAdmin")]
    [HttpPatch("{venueId:guid}/reject")]
    public async Task<IActionResult> Reject(Guid venueId)
    {
        var result = await _mediator.Send(new RejectVenueCommand(venueId));
        return Ok(result);
    }

    [Authorize(Roles = "SuperAdmin,Manager")]
    [HttpPut("{venueId:guid}")]
    public async Task<IActionResult> Update(Guid venueId, [FromBody] BidaPlatform.Application.Models.Venues.UpdateVenue.UpdateVenueRequest request)
    {
        var targetVenueId = _currentUser.Role == "Manager" ? _currentUser.VenueId ?? venueId : venueId;
        var result = await _mediator.Send(new UpdateVenueCommand(_currentUser.UserId, _currentUser.Role, _currentUser.VenueId, targetVenueId, request));
        return Ok(result);
    }
}
