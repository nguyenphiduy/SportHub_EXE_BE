using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BidaPlatform.Application.Models.Shifts.CreateShift;
using BidaPlatform.Application.UseCases.Shifts.CheckInShift;
using BidaPlatform.Application.UseCases.Shifts.CreateShift;
using BidaPlatform.Application.UseCases.Shifts.DeleteShift;
using BidaPlatform.Application.UseCases.Shifts.GetMyShifts;
using BidaPlatform.Application.UseCases.Shifts.GetShiftById;
using BidaPlatform.Application.UseCases.Shifts.GetVenueShifts;
using BidaPlatform.Application.Interfaces;

namespace BidaPlatform.Presentation.Controllers;

[ApiController]
[Route("api/shifts")]
[Authorize(Roles = "SuperAdmin,Manager,Staff")]
public class ShiftController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserContext _currentUser;

    public ShiftController(IMediator mediator, ICurrentUserContext currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Manager")]
    public async Task<IActionResult> Create([FromBody] CreateShiftRequest request)
    {
        var result = await _mediator.Send(new CreateShiftCommand(_currentUser.UserId, _currentUser.Role, request));
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpGet("venues/{venueId:guid}")]
    [Authorize(Roles = "SuperAdmin,Manager")]
    public async Task<IActionResult> GetVenueShifts(Guid venueId)
    {
        var targetVenueId = _currentUser.Role == "SuperAdmin" ? venueId : _currentUser.VenueId ?? venueId;
        var result = await _mediator.Send(new GetVenueShiftsQuery(_currentUser.UserId, _currentUser.Role, targetVenueId));
        return Ok(result);
    }

    [HttpGet("my-shifts/{userId:guid}")]
    [Authorize(Roles = "Staff")]
    public async Task<IActionResult> GetMyShifts(Guid userId)
    {
        var result = await _mediator.Send(new GetMyShiftsQuery(userId));
        return Ok(result);
    }

    [HttpGet("{shiftId:guid}")]
    public async Task<IActionResult> GetById(Guid shiftId)
    {
        var result = await _mediator.Send(new GetShiftByIdQuery(shiftId));
        return Ok(result);
    }

    [HttpDelete("{shiftId:guid}")]
    [Authorize(Roles = "SuperAdmin,Manager")]
    public async Task<IActionResult> Delete(Guid shiftId)
    {
        var result = await _mediator.Send(new DeleteShiftCommand(_currentUser.UserId, _currentUser.Role, shiftId));
        return Ok(result);
    }

    [HttpPatch("{shiftId:guid}/check-in")]
    [Authorize(Roles = "Staff")]
    public async Task<IActionResult> CheckIn(Guid shiftId)
    {
        var result = await _mediator.Send(new CheckInShiftCommand(_currentUser.UserId, _currentUser.Role, shiftId));
        return Ok(result);
    }
}
