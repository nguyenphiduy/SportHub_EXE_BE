using BidaPlatform.Application.Models.Auth.Login;
using BidaPlatform.Application.Models.Auth.Password;
using BidaPlatform.Application.Models.Auth.RefreshToken;
using BidaPlatform.Application.UseCases.Auth.ChangePassword;
using BidaPlatform.Application.UseCases.Auth.ForgotPassword;
using BidaPlatform.Application.UseCases.Auth.Login;
using BidaPlatform.Application.UseCases.Auth.Logout;
using BidaPlatform.Application.UseCases.Auth.RefreshToken;
using BidaPlatform.Application.UseCases.Auth.ResetPassword;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BidaPlatform.Presentation.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // =====================================================
    // LOGIN
    // =====================================================

    /// <summary>
    /// Login with email & password
    /// </summary>
    /// <returns>Access token (5p) & Refresh token (7 days)</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = await _mediator.Send(
                new LoginCommand(request.Email, request.Password));

            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    // =====================================================
    // REFRESH TOKEN (ROTATE)
    // =====================================================

    /// <summary>
    /// Refresh access token using refresh token (rotate & revoke old)
    /// </summary>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(RefreshTokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken(
        [FromBody] RefreshTokenRequest request)
    {
        try
        {
            var result = await _mediator.Send(
                new RefreshTokenCommand(request.RefreshToken));

            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    // =====================================================
    // LOGOUT (REVOKE REFRESH TOKEN)
    // =====================================================

    /// <summary>
    /// Logout and revoke refresh token
    /// </summary>
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout(
        [FromBody] LogoutRequest request)
    {
        await _mediator.Send(
            new LogoutCommand(request.RefreshToken));

        return NoContent();
    }

    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(
    [FromBody] ForgotPasswordRequest request)
    {
        await _mediator.Send(
            new ForgotPasswordCommand(request.Email));

        return Ok(new
        {
            message = "Nếu email tồn tại, link reset đã được gửi."
        });
    }


    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(
    [FromBody] ResetPasswordRequest request)
    {
        if (request == null)
            return BadRequest(new { message = "Invalid request body" });

        await _mediator.Send(
            new ResetPasswordCommand(
                request.Token,
                request.NewPassword));

        return NoContent();
    }



    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(
    [FromBody] ChangePasswordRequest request)
    {
        var userId = Guid.Parse(
            User.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

        await _mediator.Send(
            new ChangePasswordCommand(
                userId,
                request.CurrentPassword,
                request.NewPassword));

        return NoContent();
    }


}
