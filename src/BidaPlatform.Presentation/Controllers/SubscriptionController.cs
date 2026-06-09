using BidaPlatform.Application.Models.Subscriptions.AssignPlan;
using BidaPlatform.Application.Models.Subscriptions.UpdatePlan;
using BidaPlatform.Application.UseCases.Subscriptions.AssignSubscription;
using BidaPlatform.Application.UseCases.Subscriptions.GetCurrentSubscription;
using BidaPlatform.Application.UseCases.Subscriptions.GetSubscriptionHistory;
using BidaPlatform.Application.UseCases.Subscriptions.UpdateSubscription;
using BidaPlatform.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BidaPlatform.Presentation.Controllers;

[ApiController]
[Route("api/subscriptions")]
public class SubscriptionController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserContext _currentUser;

    public SubscriptionController(IMediator mediator, ICurrentUserContext currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    [Authorize(Roles = "SuperAdmin")]
    [HttpPost("venues/{venueId:guid}")]
    public async Task<IActionResult> Assign(Guid venueId, [FromBody] AssignSubscriptionRequest request)
    {
        var result = await _mediator.Send(new AssignSubscriptionCommand(_currentUser.UserId, venueId, request));
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [Authorize(Roles = "SuperAdmin")]
    [HttpPut("{subscriptionId:guid}")]
    public async Task<IActionResult> Update(Guid subscriptionId, [FromBody] UpdateSubscriptionRequest request)
    {
        var result = await _mediator.Send(new UpdateSubscriptionCommand(subscriptionId, request));
        return Ok(result);
    }

    [Authorize(Roles = "SuperAdmin,Manager")]
    [HttpGet("venues/{venueId:guid}/current")]
    public async Task<IActionResult> GetCurrent(Guid venueId)
    {
        var targetVenueId = _currentUser.Role == "Manager" ? _currentUser.VenueId ?? venueId : venueId;
        var result = await _mediator.Send(new GetCurrentSubscriptionQuery(targetVenueId));
        return Ok(result);
    }

    [Authorize(Roles = "SuperAdmin,Manager")]
    [HttpGet("venues/{venueId:guid}/history")]
    public async Task<IActionResult> GetHistory(Guid venueId)
    {
        var targetVenueId = _currentUser.Role == "Manager" ? _currentUser.VenueId ?? venueId : venueId;
        var result = await _mediator.Send(new GetSubscriptionHistoryQuery(targetVenueId));
        return Ok(result);
    }
}
