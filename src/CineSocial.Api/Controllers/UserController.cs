using CineSocial.Application.Features.Users.Commands.UpdateBackgroundImage;
using CineSocial.Application.Features.Users.Commands.UpdateProfile;
using CineSocial.Application.Features.Users.Commands.UpdateProfileImage;
using CineSocial.Application.Features.Users.Queries.GetCurrent;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CineSocial.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var query = new GetCurrentUserQuery(userId);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileCommand command)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        if (command.UserId != userId)
            return Forbid();

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("profile-image")]
    public async Task<IActionResult> UpdateProfileImage(IFormFile file)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        if (file == null || file.Length == 0)
            return BadRequest(new { isSuccess = false, message = "No file uploaded" });

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);

        var command = new UpdateProfileImageCommand(
            userId,
            file.FileName,
            file.ContentType,
            memoryStream.ToArray()
        );

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("background-image")]
    public async Task<IActionResult> UpdateBackgroundImage(IFormFile file)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        if (file == null || file.Length == 0)
            return BadRequest(new { isSuccess = false, message = "No file uploaded" });

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);

        var command = new UpdateBackgroundImageCommand(
            userId,
            file.FileName,
            file.ContentType,
            memoryStream.ToArray()
        );

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("admin-only")]
    [Authorize(Roles = "Admin,SuperUser")]
    public IActionResult AdminOnly()
    {
        return Ok(new { Message = "This endpoint is only for Admins and SuperUsers" });
    }

    [HttpGet("superuser-only")]
    [Authorize(Roles = "SuperUser")]
    public IActionResult SuperUserOnly()
    {
        return Ok(new { Message = "This endpoint is only for SuperUsers" });
    }
}
