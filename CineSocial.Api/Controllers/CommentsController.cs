using System.Security.Claims;
using CineSocial.Api.DTOs;
using CineSocial.Core.Features.Comments.Commands;
using CineSocial.Core.Features.Comments.Queries;
using CineSocial.Core.Features.Reactions.Commands;
using CineSocial.Core.Features.Reactions.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace CineSocial.Api.Controllers;

/// <summary>
/// Comment management endpoints for reviews
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CommentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CommentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get comments for a specific review
    /// </summary>
    /// <param name="reviewId">Review ID</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Items per page (default: 10, max: 50)</param>
    /// <param name="sortBy">Sort field (created_at, upvotes, updated_at)</param>
    /// <param name="sortOrder">Sort order (asc, desc)</param>
    /// <returns>Paginated list of comments with nested replies</returns>
    /// <response code="200">Comments retrieved successfully</response>
    /// <response code="404">Review not found</response>
    [HttpGet("review/{reviewId:guid}")]
    [SwaggerResponseExample(200, typeof(Swagger.Examples.CommentDtoExample))]
    [ProducesResponseType(typeof(List<CommentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetReviewComments(
        Guid reviewId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string sortBy = "created_at",
        [FromQuery] string sortOrder = "asc")
    {
        pageSize = Math.Min(pageSize, 50);

        var query = new GetReviewCommentsQuery(reviewId, page, pageSize, sortBy, sortOrder);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Error });
        }

        var response = result.Data!.Comments.Select(c => new CommentDto
        {
            Id = c.Id,
            ReviewId = c.ReviewId,
            UserId = c.UserId,
            Username = c.UserUsername,
            Content = c.Content,
            UpvotesCount = c.UpvotesCount,
            DownvotesCount = c.DownvotesCount,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt,
            ParentCommentId = c.ParentCommentId,
            Replies = c.Replies.Select(r => new CommentDto
            {
                Id = r.Id,
                ReviewId = r.ReviewId,
                UserId = r.UserId,
                Username = r.UserUsername,
                Content = r.Content,
                UpvotesCount = r.UpvotesCount,
                DownvotesCount = r.DownvotesCount,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt,
                ParentCommentId = r.ParentCommentId,
                Replies = new List<CommentDto>()
            }).ToList()
        }).ToList();

        var paginationResponse = new
        {
            data = response,
            pagination = new
            {
                totalCount = result.Data.TotalCount,
                totalPages = result.Data.TotalPages,
                currentPage = result.Data.CurrentPage,
                pageSize = result.Data.PageSize
            }
        };

        return Ok(paginationResponse);
    }

    /// <summary>
    /// Get a specific comment by ID
    /// </summary>
    /// <param name="id">Comment ID</param>
    /// <returns>Comment details</returns>
    /// <response code="200">Comment found</response>
    /// <response code="404">Comment not found</response>
    [HttpGet("{id:guid}")]
    [SwaggerResponseExample(200, typeof(Swagger.Examples.CommentDtoExample))]
    [ProducesResponseType(typeof(CommentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetComment(Guid id)
    {
        var query = new GetCommentByIdQuery(id);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Error });
        }

        if (result.Data == null)
        {
            return NotFound(new { message = "Yorum bulunamadı." });
        }

        var response = new CommentDto
        {
            Id = result.Data.Id,
            ReviewId = result.Data.ReviewId,
            UserId = result.Data.UserId,
            Username = result.Data.UserUsername,
            Content = result.Data.Content,
            UpvotesCount = result.Data.UpvotesCount,
            DownvotesCount = result.Data.DownvotesCount,
            CreatedAt = result.Data.CreatedAt,
            UpdatedAt = result.Data.UpdatedAt,
            ParentCommentId = result.Data.ParentCommentId,
            Replies = new List<CommentDto>()
        };

        return Ok(response);
    }

    /// <summary>
    /// Create a new comment on a review
    /// </summary>
    /// <param name="request">Comment information</param>
    /// <returns>Created comment</returns>
    /// <response code="201">Comment created successfully</response>
    /// <response code="400">Invalid comment data</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="404">Review or parent comment not found</response>
    [HttpPost]
    [Authorize]
    [SwaggerRequestExample(typeof(CreateCommentRequest), typeof(Swagger.Examples.CreateCommentRequestExample))]
    [SwaggerResponseExample(201, typeof(Swagger.Examples.CommentDtoExample))]
    [ProducesResponseType(typeof(CommentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateComment([FromBody] CreateCommentRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Geçersiz token." });
        }

        var command = new CreateCommentCommand(
            userId,
            request.ReviewId,
            request.Content,
            request.ParentCommentId
        );

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Error });
        }

        var response = new CommentDto
        {
            Id = result.Data!.Id,
            ReviewId = result.Data.ReviewId,
            UserId = result.Data.UserId,
            Username = result.Data.UserUsername,
            Content = result.Data.Content,
            UpvotesCount = result.Data.UpvotesCount,
            DownvotesCount = result.Data.DownvotesCount,
            CreatedAt = result.Data.CreatedAt,
            UpdatedAt = result.Data.UpdatedAt,
            ParentCommentId = result.Data.ParentCommentId,
            Replies = new List<CommentDto>()
        };

        return CreatedAtAction(nameof(GetComment), new { id = response.Id }, response);
    }

    /// <summary>
    /// Update an existing comment
    /// </summary>
    /// <param name="id">Comment ID</param>
    /// <param name="request">Updated comment information</param>
    /// <returns>Updated comment</returns>
    /// <response code="200">Comment updated successfully</response>
    /// <response code="400">Invalid comment data</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="403">User not authorized to update this comment</response>
    /// <response code="404">Comment not found</response>
    [HttpPut("{id:guid}")]
    [Authorize]
    [SwaggerRequestExample(typeof(UpdateCommentRequest), typeof(Swagger.Examples.UpdateCommentRequestExample))]
    [SwaggerResponseExample(200, typeof(Swagger.Examples.CommentDtoExample))]
    [ProducesResponseType(typeof(CommentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateComment(Guid id, [FromBody] UpdateCommentRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Geçersiz token." });
        }

        var command = new UpdateCommentCommand(id, userId, request.Content);
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
        {
            if (result.Error!.Contains("yetkiniz yok"))
            {
                return StatusCode(403, new { message = result.Error });
            }
            if (result.Error.Contains("bulunamadı"))
            {
                return NotFound(new { message = result.Error });
            }
            return BadRequest(new { message = result.Error });
        }

        var response = new CommentDto
        {
            Id = result.Data!.Id,
            ReviewId = result.Data.ReviewId,
            UserId = result.Data.UserId,
            Username = result.Data.UserUsername,
            Content = result.Data.Content,
            UpvotesCount = result.Data.UpvotesCount,
            DownvotesCount = result.Data.DownvotesCount,
            CreatedAt = result.Data.CreatedAt,
            UpdatedAt = result.Data.UpdatedAt,
            ParentCommentId = result.Data.ParentCommentId,
            Replies = new List<CommentDto>()
        };

        return Ok(response);
    }

    /// <summary>
    /// Delete a comment
    /// </summary>
    /// <param name="id">Comment ID</param>
    /// <returns>Confirmation of deletion</returns>
    /// <response code="204">Comment deleted successfully</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="403">User not authorized to delete this comment</response>
    /// <response code="404">Comment not found</response>
    [HttpDelete("{id:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteComment(Guid id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Geçersiz token." });
        }

        var command = new DeleteCommentCommand(id, userId);
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
        {
            if (result.Error!.Contains("yetkiniz yok"))
            {
                return StatusCode(403, new { message = result.Error });
            }
            if (result.Error.Contains("bulunamadı"))
            {
                return NotFound(new { message = result.Error });
            }
            return BadRequest(new { message = result.Error });
        }

        return NoContent();
    }

    /// <summary>
    /// React to a comment (upvote or downvote)
    /// </summary>
    /// <param name="request">Reaction information</param>
    /// <returns>Created or updated reaction</returns>
    /// <response code="200">Reaction updated successfully</response>
    /// <response code="201">Reaction created successfully</response>
    /// <response code="400">Invalid reaction data</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="404">Comment not found</response>
    [HttpPost("reaction")]
    [Authorize]
    [SwaggerRequestExample(typeof(CreateReactionRequest), typeof(Swagger.Examples.CreateReactionRequestExample))]
    [SwaggerResponseExample(201, typeof(Swagger.Examples.ReactionDtoExample))]
    [ProducesResponseType(typeof(ReactionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ReactionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReactToComment([FromBody] CreateReactionRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Geçersiz token." });
        }

        var command = new CreateOrUpdateReactionCommand(userId, request.CommentId, request.Type);
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Error });
        }

        var response = new ReactionDto
        {
            Id = result.Data!.Id,
            UserId = result.Data.UserId,
            CommentId = result.Data.CommentId,
            Type = result.Data.Type,
            CreatedAt = result.Data.CreatedAt,
            UpdatedAt = result.Data.UpdatedAt
        };

        return result.Data.UpdatedAt.HasValue ? Ok(response) : Created("", response);
    }

    /// <summary>
    /// Get user's reaction for a specific comment
    /// </summary>
    /// <param name="commentId">Comment ID</param>
    /// <returns>User's reaction for the comment</returns>
    /// <response code="200">Reaction found</response>
    /// <response code="404">Reaction not found</response>
    /// <response code="401">User not authenticated</response>
    [HttpGet("reaction/comment/{commentId:guid}")]
    [Authorize]
    [SwaggerResponseExample(200, typeof(Swagger.Examples.ReactionDtoExample))]
    [ProducesResponseType(typeof(ReactionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUserReaction(Guid commentId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Geçersiz token." });
        }

        var query = new GetUserReactionQuery(userId, commentId);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Error });
        }

        if (result.Data == null)
        {
            return NotFound(new { message = "Bu yorum için tepkiniz bulunamadı." });
        }

        var response = new ReactionDto
        {
            Id = result.Data.Id,
            UserId = result.Data.UserId,
            CommentId = result.Data.CommentId,
            Type = result.Data.Type,
            CreatedAt = result.Data.CreatedAt,
            UpdatedAt = result.Data.UpdatedAt
        };

        return Ok(response);
    }

    /// <summary>
    /// Get comment reaction statistics
    /// </summary>
    /// <param name="commentId">Comment ID</param>
    /// <returns>Reaction statistics for the comment</returns>
    /// <response code="200">Reaction stats retrieved successfully</response>
    /// <response code="404">Comment not found</response>
    [HttpGet("reaction/comment/{commentId:guid}/stats")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCommentReactionStats(Guid commentId)
    {
        var query = new GetCommentReactionsQuery(commentId);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Error });
        }

        var response = new
        {
            CommentId = result.Data!.CommentId,
            UpvotesCount = result.Data.UpvotesCount,
            DownvotesCount = result.Data.DownvotesCount,
            TotalReactions = result.Data.TotalReactions
        };

        return Ok(response);
    }

    /// <summary>
    /// Delete user's reaction for a comment
    /// </summary>
    /// <param name="commentId">Comment ID</param>
    /// <returns>Confirmation of deletion</returns>
    /// <response code="204">Reaction deleted successfully</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="404">Reaction not found</response>
    [HttpDelete("reaction/comment/{commentId:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteReaction(Guid commentId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Geçersiz token." });
        }

        var command = new DeleteReactionCommand(userId, commentId);
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Error });
        }

        return NoContent();
    }
}