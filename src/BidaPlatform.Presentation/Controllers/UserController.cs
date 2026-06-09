using BidaPlatform.Application.Models.Users.CreateUser;
using BidaPlatform.Application.Models.Users.CreateManager;
using BidaPlatform.Application.Models.Users.UpdateUser;
using BidaPlatform.Application.UseCases.Users.CreateUser;
using BidaPlatform.Application.UseCases.Users.CreateManager;
using BidaPlatform.Application.UseCases.Users.ReadAllUser;
using BidaPlatform.Application.UseCases.Users.ReadMyProfile;
using BidaPlatform.Application.UseCases.Users.ToggleActive;
using BidaPlatform.Application.UseCases.Users.UpdateUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BidaPlatform.Presentation.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// SuperAdmin creates Manager account and a new venue together.
    /// </summary>
    [Authorize(Roles = "SuperAdmin")]
    [HttpPost("create-manager-with-venue")]
    public async Task<IActionResult> CreateManagerWithVenue([FromBody] CreateManagerWithVenueRequest request)
    {
        try
        {
            await _mediator.Send(
                new CreateManagerWithVenueCommand(
                    request.Email,
                    request.FullName,
                    request.VenueName,
                    request.Address,
                    request.Phone,
                    request.OwnerName));

            return StatusCode(StatusCodes.Status201Created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// SuperAdmin creates Manager account for an existing venue.
    /// Password will be auto-generated and sent via email.
    /// </summary>
    [Authorize(Roles = "SuperAdmin")]
    [HttpPost("create-manager")]
    public async Task<IActionResult> CreateManager([FromBody] CreateManagerRequest request)
    {
        try
        {
            await _mediator.Send(
                new CreateManagerCommand(
                    request.Email,
                    request.FullName));

            return StatusCode(StatusCodes.Status201Created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Manager creates staff account for their venue.
    /// Password will be auto-generated and sent via email.
    /// </summary>
    [Authorize(Roles = "Manager")]
    [HttpPost("create-staff")]
    public async Task<IActionResult> CreateStaff(
        [FromBody] CreateUserRequest request)
    {
        try
        {
            var creatorId = Guid.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            await _mediator.Send(
                new CreateUserCommand(
                    creatorId,
                    request.Email,
                    request.FullName,
                    request.VenueId));

            return StatusCode(StatusCodes.Status201Created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// SuperAdmin sees all users. Manager sees users in their venue.
    /// </summary>
    [Authorize(Roles = "SuperAdmin,Manager")]
    [HttpGet]
    public async Task<IActionResult> ReadAllUsers([FromQuery] string? role)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var roleClaim = User.FindFirst("role");

        if (userIdClaim == null || roleClaim == null)
            return Unauthorized("Token không hợp lệ");

        var userId = Guid.Parse(userIdClaim.Value);
        var userRole = roleClaim.Value;

        var result = await _mediator.Send(
            new ReadAllUserQuery(userId, userRole, role));

        return Ok(result);
    }

    /// <summary>
    /// User updates their own profile (Email / FullName).
    /// SuperAdmin is not allowed here.
    /// </summary>
    [Authorize]
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile(
        [FromBody] UpdateProfileRequest request)
    {
        var userId = Guid.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var role = User.FindFirst("role")!.Value;

        await _mediator.Send(
            new UpdateProfileCommand(
                userId,
                role,
                request.Email,
                request.FullName));

        return NoContent();
    }

    /// <summary>
    /// SuperAdmin bật / tắt trạng thái bất kỳ user nào.
    /// Manager bật / tắt trạng thái staff thuộc quán của mình.
    /// </summary>
    [Authorize(Roles = "SuperAdmin,Manager")]
    [HttpPatch("{userId}/active")]
    public async Task<IActionResult> ToggleActive(
        Guid userId,
        [FromQuery] bool isActive)
    {
        var requesterId = Guid.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var role = User.FindFirst("role")!.Value;

        await _mediator.Send(
            new ToggleUserActiveCommand(
                requesterId,
                userId,
                isActive,
                role));

        return NoContent();
    }

    /// <summary>
    /// Get my own profile
    /// </summary>
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
            return Unauthorized("Token không hợp lệ");

        var userId = Guid.Parse(userIdClaim.Value);

        var result = await _mediator.Send(
            new ReadMyProfileQuery(userId));

        return Ok(result);
    }
}
