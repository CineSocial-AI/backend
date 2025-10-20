using CineSocial.Application.Features.Blocks.Commands.Block;
using CineSocial.Application.Features.Blocks.Commands.Unblock;
using CineSocial.Application.Features.Blocks.Queries.GetBlockedUsers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineSocial.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BlockController : ControllerBase
{
    private readonly IMediator _mediator;

    public BlockController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("block")]
    public async Task<IActionResult> Block([FromBody] BlockCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpPost("unblock")]
    public async Task<IActionResult> Unblock([FromBody] UnblockCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpGet("blocked-users")]
    public async Task<IActionResult> GetBlockedUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetBlockedUsersQuery(pageNumber, pageSize);
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}
