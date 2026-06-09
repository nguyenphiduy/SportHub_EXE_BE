using System.Security.Claims;
using BidaPlatform.Application.Models.Tables.CreateTable;
using BidaPlatform.Application.Models.Tables.UpdateTable;
using BidaPlatform.Application.UseCases.Sessions.UpdatePayment;
using BidaPlatform.Application.UseCases.Sessions.ViewSessions;
using BidaPlatform.Application.UseCases.Sessions.ViewTableSessions;
using BidaPlatform.Application.UseCases.Tables.CreateTable;
using BidaPlatform.Application.UseCases.Tables.DeleteTable;
using BidaPlatform.Application.UseCases.Tables.RestoreTable;
using BidaPlatform.Application.UseCases.Tables.StartSession;
using BidaPlatform.Application.UseCases.Tables.StopSession;
using BidaPlatform.Application.UseCases.Tables.PingDevice;
using BidaPlatform.Application.UseCases.Tables.UpdateTable;
using BidaPlatform.Application.UseCases.Tables.ViewTables;
using BidaPlatform.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BidaPlatform.Presentation.Controllers;

[ApiController]
[Route("api/tables")]
[Authorize(Roles = "SuperAdmin,Manager,Staff")]
public class TablesController : ControllerBase
{
    private readonly IMediator _mediator;

    public TablesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid venueId, [FromQuery] bool activeOnly = false)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _mediator.Send(new ReadAllTablesQuery(userId, venueId, activeOnly));
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Manager")]
    public async Task<IActionResult> Create([FromQuery] Guid venueId, [FromBody] CreateTableRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _mediator.Send(new CreateTableCommand(
            userId,
            venueId,
            request.Name,
            request.Type,
            request.PricePerHour,
            request.DeviceIpAddress,
            request.DeviceName));
        return StatusCode(201);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "SuperAdmin,Manager")]
    public async Task<IActionResult> Update(Guid id, [FromQuery] Guid venueId, [FromBody] UpdateTableRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _mediator.Send(new UpdateTableCommand(userId, venueId, id, request));
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "SuperAdmin,Manager")]
    public async Task<IActionResult> Delete(Guid id, [FromQuery] Guid venueId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _mediator.Send(new DeleteTableCommand(userId, venueId, id));
        return NoContent();
    }

    [HttpPatch("{id:guid}/restore")]
    [Authorize(Roles = "SuperAdmin,Manager")]
    public async Task<IActionResult> Restore(Guid id, [FromQuery] Guid venueId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _mediator.Send(new RestoreTableCommand(userId, venueId, id));
        return NoContent();
    }

    [HttpPost("{id:guid}/start")]
    public async Task<IActionResult> StartSession(Guid id, [FromQuery] Guid venueId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _mediator.Send(new StartSessionCommand(id, userId));
        return Ok(new { message = "Đã bật đèn và bắt đầu session." });
    }

    [HttpPost("{id:guid}/stop")]
    public async Task<IActionResult> StopSession(Guid id, [FromQuery] Guid venueId, [FromBody] StopSessionBody? body = null)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _mediator.Send(new StopSessionCommand(userId, venueId, id, body?.Note, body?.PaymentMethod));
        return Ok(result);
    }

    public record StopSessionBody(string? Note, BilliardPaymentMethod? PaymentMethod = null);

    [HttpPatch("sessions/{sessionId:guid}/payment")]
    public async Task<IActionResult> UpdateSessionPayment(Guid sessionId, [FromBody] UpdatePaymentBody? body)
    {
        if (body is null) return BadRequest("Request body is required.");
        await _mediator.Send(new UpdateSessionPaymentCommand(sessionId, body.PaymentMethod, body.MarkAsPaid));
        return NoContent();
    }

    public record UpdatePaymentBody(BilliardPaymentMethod? PaymentMethod = null, bool MarkAsPaid = false);

    [HttpPost("{id:guid}/device/ping")]
    public async Task<IActionResult> PingDevice(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new PingDeviceCommand(id));
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("sessions")]
    public async Task<IActionResult> GetAllSessions([FromQuery] Guid venueId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _mediator.Send(new ReadAllSessionsQuery(userId, venueId));
        return Ok(result);
    }

    [HttpGet("{id:guid}/sessions")]
    public async Task<IActionResult> GetTableSessions(Guid id, [FromQuery] Guid venueId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _mediator.Send(new ReadTableSessionsQuery(userId, venueId, id));
        return Ok(result);
    }
}
