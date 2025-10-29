using CineSocial.Application.Features.Comments.Commands.CreateComment;
using CineSocial.Application.Features.Comments.Commands.DeleteComment;
using CineSocial.Application.Features.Comments.Commands.ReplyToComment;
using CineSocial.Application.Features.Comments.Commands.UpdateComment;
using CineSocial.Application.Features.Comments.Queries.GetMovieComments;
using CineSocial.Application.Features.Comments.Queries.GetCommentById;
using CineSocial.Application.Features.Comments.Queries.GetCommentReplies;
using CineSocial.Application.Features.Comments.Queries.GetUserComments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineSocial.Api.Controllers;

/// <summary>
/// Comment management endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CommentController : ControllerBase
{
    private readonly IMediator _mediator;

    public CommentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all comments for a movie (root comments only)
    /// </summary>
    [HttpGet("movie/{movieId}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetMovieComments(int movieId)
    {
        var query = new GetMovieCommentsQuery(movieId);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result.Data);
    }

    /// <summary>
    /// Get a comment by ID with all its details
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetComment(int id)
    {
        var query = new GetCommentByIdQuery(id);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result.Data);
    }

    /// <summary>
    /// Get all replies for a specific comment
    /// </summary>
    [HttpGet("{id}/replies")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCommentReplies(int id)
    {
        var query = new GetCommentRepliesQuery(id);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result.Data);
    }

    /// <summary>
    /// Get all comments by a user
    /// </summary>
    [HttpGet("user/{userId}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetUserComments(int userId)
    {
        var query = new GetUserCommentsQuery(userId);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result.Data);
    }

    /// <summary>
    /// Create a new comment
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateComment([FromBody] CreateCommentCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess ? CreatedAtAction(nameof(GetComment), new { id = result.Data }, result) : BadRequest(result);
    }

    /// <summary>
    /// Reply to an existing comment
    /// </summary>
    [HttpPost("reply")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ReplyToComment([FromBody] ReplyToCommentCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess ? CreatedAtAction(nameof(GetComment), new { id = result.Data }, result) : BadRequest(result);
    }

    /// <summary>
    /// Update an existing comment
    /// </summary>
    [HttpPut("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateComment(int id, [FromBody] UpdateCommentCommand command)
    {
        if (id != command.CommentId)
            return BadRequest(new { message = "Comment ID mismatch" });

        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Delete a comment (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteComment(int id)
    {
        var command = new DeleteCommentCommand(id);
        var result = await _mediator.Send(command);
        return result.IsSuccess ? NoContent() : BadRequest(result);
    }
}
