using CineSocial.Application.Features.Reactions.Commands.AddReaction;
using CineSocial.Application.Features.Reactions.Commands.RemoveReaction;
using CineSocial.Application.Features.Reactions.Queries.GetCommentReactions;
using CineSocial.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineSocial.Api.Controllers;

/// <summary>
/// Comment reaction management endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ReactionController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReactionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all reactions for a comment
    /// </summary>
    [HttpGet("comment/{commentId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCommentReactions(int commentId)
    {
        var query = new GetCommentReactionsQuery(commentId);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result.Data);
    }

    /// <summary>
    /// Add a reaction to a comment (upvote or downvote)
    /// </summary>
    [HttpPost("comment/{commentId}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddReaction(
        int commentId,
        [FromBody] AddReactionRequest request)
    {
        var command = new AddReactionCommand(commentId, request.Type);
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Remove a reaction from a comment
    /// </summary>
    [HttpDelete("comment/{commentId}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RemoveReaction(int commentId)
    {
        var command = new RemoveReactionCommand(commentId);
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return NoContent();
    }
}

// Request DTOs
public record AddReactionRequest(ReactionType Type);
