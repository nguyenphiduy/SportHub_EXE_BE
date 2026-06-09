using BidaPlatform.Application.Models.AI;
using BidaPlatform.Application.UseCases.AI.GetAiProviderSettings;
using BidaPlatform.Application.UseCases.AI.UpdateAiProviderSettings;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BidaPlatform.Presentation.Controllers;

[ApiController]
[Route("api/admin/ai")]
[Authorize(Roles = "SuperAdmin")]
public class AdminAiController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminAiController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("settings")]
    public async Task<IActionResult> GetSettings()
    {
        var result = await _mediator.Send(new GetAiProviderSettingsQuery());
        return Ok(result);
    }

    [HttpPut("settings")]
    public async Task<IActionResult> UpdateSettings([FromBody] UpdateAiProviderSettingsRequest request)
    {
        var actorUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _mediator.Send(new UpdateAiProviderSettingsCommand(actorUserId, request));
        return Ok(result);
    }
}
