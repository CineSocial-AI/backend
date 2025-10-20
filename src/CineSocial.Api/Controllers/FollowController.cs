using CineSocial.Application.Features.Follows.Commands.Follow;
using CineSocial.Application.Features.Follows.Commands.Unfollow;
using CineSocial.Application.Features.Follows.Queries.GetFollowers;
using CineSocial.Application.Features.Follows.Queries.GetFollowing;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineSocial.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FollowController : ControllerBase
{
    private readonly IMediator _mediator;

    public FollowController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("follow")]
    public async Task<IActionResult> Follow([FromBody] FollowCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpPost("unfollow")]
    public async Task<IActionResult> Unfollow([FromBody] UnfollowCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpGet("followers/{userId}")]
    public async Task<IActionResult> GetFollowers(int userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetFollowersQuery(userId, pageNumber, pageSize);
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpGet("following/{userId}")]
    public async Task<IActionResult> GetFollowing(int userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetFollowingQuery(userId, pageNumber, pageSize);
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}
