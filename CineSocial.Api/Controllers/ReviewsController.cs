using System.Security.Claims;
using CineSocial.Api.DTOs;
using CineSocial.Core.Features.Ratings.Commands;
using CineSocial.Core.Features.Ratings.Queries;
using CineSocial.Core.Features.Reviews.Commands;
using CineSocial.Core.Features.Reviews.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace CineSocial.Api.Controllers;

/// <summary>
/// Movie reviews and ratings endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ReviewsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReviewsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    /// <summary>
    /// Get reviews for a specific movie
    /// </summary>
    /// <param name="movieId">Movie ID</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Items per page (default: 10, max: 50)</param>
    /// <param name="sortBy">Sort field (created_at, likes_count, updated_at)</param>
    /// <param name="sortOrder">Sort order (asc, desc)</param>
    /// <returns>Paginated list of movie reviews</returns>
    /// <response code="200">Reviews retrieved successfully</response>
    /// <response code="404">Movie not found</response>
    [HttpGet("movie/{movieId:guid}")]
    [SwaggerResponseExample(200, typeof(Swagger.Examples.ReviewDtoExample))]
    [ProducesResponseType(typeof(List<ReviewDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMovieReviews(
        Guid movieId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string sortBy = "created_at",
        [FromQuery] string sortOrder = "desc")
    {
        pageSize = Math.Min(pageSize, 50);

        var query = new GetMovieReviewsQuery(movieId, page, pageSize, sortBy, sortOrder);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Error });
        }

        var response = result.Data!.Reviews.Select(r => new ReviewDto
        {
            Id = r.Id,
            UserId = r.UserId,
            MovieId = r.MovieId,
            Title = r.Title,
            Content = r.Content,
            ContainsSpoilers = r.ContainsSpoilers,
            LikesCount = r.LikesCount,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt,
            UserFullName = r.UserFullName,
            UserUsername = r.UserUsername
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
    /// Get a specific review by ID
    /// </summary>
    /// <param name="id">Review ID</param>
    /// <returns>Review details with comments</returns>
    /// <response code="200">Review found</response>
    /// <response code="404">Review not found</response>
    [HttpGet("{id:guid}")]
    [SwaggerResponseExample(200, typeof(Swagger.Examples.ReviewDtoExample))]
    [ProducesResponseType(typeof(ReviewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetReview(Guid id)
    {
        var query = new GetReviewByIdQuery(id);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Error });
        }

        if (result.Data == null)
        {
            return NotFound(new { message = "Değerlendirme bulunamadı." });
        }

        var response = new ReviewDto
        {
            Id = result.Data.Id,
            UserId = result.Data.UserId,
            MovieId = result.Data.MovieId,
            Title = result.Data.Title,
            Content = result.Data.Content,
            ContainsSpoilers = result.Data.ContainsSpoilers,
            LikesCount = result.Data.LikesCount,
            CreatedAt = result.Data.CreatedAt,
            UpdatedAt = result.Data.UpdatedAt,
            UserFullName = result.Data.UserFullName,
            UserUsername = result.Data.UserUsername
        };

        return Ok(response);
    }

    /// <summary>
    /// Create a new movie review
    /// </summary>
    /// <param name="request">Review information</param>
    /// <returns>Created review</returns>
    /// <response code="201">Review created successfully</response>
    /// <response code="400">Invalid review data</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="409">User already reviewed this movie</response>
    [HttpPost]
    [Authorize]
    [SwaggerRequestExample(typeof(CreateReviewRequest), typeof(Swagger.Examples.CreateReviewRequestExample))]
    [SwaggerResponseExample(201, typeof(Swagger.Examples.ReviewDtoExample))]
    [ProducesResponseType(typeof(ReviewDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateReview([FromBody] CreateReviewRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Geçersiz token." });
        }

        var command = new CreateReviewCommand(
            userId,
            request.MovieId,
            request.Title,
            request.Content,
            request.ContainsSpoilers
        );

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Error });
        }

        var response = new ReviewDto
        {
            Id = result.Data!.Id,
            UserId = result.Data.UserId,
            MovieId = result.Data.MovieId,
            Title = result.Data.Title,
            Content = result.Data.Content,
            ContainsSpoilers = result.Data.ContainsSpoilers,
            LikesCount = result.Data.LikesCount,
            CreatedAt = result.Data.CreatedAt,
            UpdatedAt = result.Data.UpdatedAt,
            UserFullName = result.Data.UserFullName,
            UserUsername = result.Data.UserUsername
        };

        return CreatedAtAction(nameof(GetReview), new { id = response.Id }, response);
    }

    /// <summary>
    /// Update an existing review
    /// </summary>
    /// <param name="id">Review ID</param>
    /// <param name="request">Updated review information</param>
    /// <returns>Updated review</returns>
    /// <response code="200">Review updated successfully</response>
    /// <response code="400">Invalid review data</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="403">User not authorized to update this review</response>
    /// <response code="404">Review not found</response>
    [HttpPut("{id:guid}")]
    [Authorize]
    [SwaggerRequestExample(typeof(UpdateReviewRequest), typeof(Swagger.Examples.UpdateReviewRequestExample))]
    [SwaggerResponseExample(200, typeof(Swagger.Examples.ReviewDtoExample))]
    [ProducesResponseType(typeof(ReviewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateReview(Guid id, [FromBody] UpdateReviewRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Geçersiz token." });
        }

        var command = new UpdateReviewCommand(
            id,
            userId,
            request.Title,
            request.Content,
            request.ContainsSpoilers
        );

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

        var response = new ReviewDto
        {
            Id = result.Data!.Id,
            UserId = result.Data.UserId,
            MovieId = result.Data.MovieId,
            Title = result.Data.Title,
            Content = result.Data.Content,
            ContainsSpoilers = result.Data.ContainsSpoilers,
            LikesCount = result.Data.LikesCount,
            CreatedAt = result.Data.CreatedAt,
            UpdatedAt = result.Data.UpdatedAt,
            UserFullName = result.Data.UserFullName,
            UserUsername = result.Data.UserUsername
        };

        return Ok(response);
    }

    /// <summary>
    /// Delete a review
    /// </summary>
    /// <param name="id">Review ID</param>
    /// <returns>Confirmation of deletion</returns>
    /// <response code="204">Review deleted successfully</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="403">User not authorized to delete this review</response>
    /// <response code="404">Review not found</response>
    [HttpDelete("{id:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteReview(Guid id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Geçersiz token." });
        }

        var command = new DeleteReviewCommand(id, userId);
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
    /// Rate a movie
    /// </summary>
    /// <param name="request">Rating information</param>
    /// <returns>Created or updated rating</returns>
    /// <response code="200">Rating updated successfully</response>
    /// <response code="201">Rating created successfully</response>
    /// <response code="400">Invalid rating data</response>
    /// <response code="401">User not authenticated</response>
    [HttpPost("rating")]
    [Authorize]
    [SwaggerRequestExample(typeof(CreateRatingRequest), typeof(Swagger.Examples.CreateRatingRequestExample))]
    [SwaggerResponseExample(201, typeof(Swagger.Examples.RatingDtoExample))]
    [ProducesResponseType(typeof(RatingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RatingDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RateMovie([FromBody] CreateRatingRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Geçersiz token." });
        }

        var command = new CreateOrUpdateRatingCommand(userId, request.MovieId, request.Score);
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Error });
        }

        var response = new RatingDto
        {
            Id = result.Data!.Id,
            UserId = result.Data.UserId,
            MovieId = result.Data.MovieId,
            Score = result.Data.Score,
            CreatedAt = result.Data.CreatedAt,
            UpdatedAt = result.Data.UpdatedAt
        };

        return result.Data.UpdatedAt.HasValue ? Ok(response) : Created("", response);
    }

    /// <summary>
    /// Get user's rating for a specific movie
    /// </summary>
    /// <param name="movieId">Movie ID</param>
    /// <returns>User's rating for the movie</returns>
    /// <response code="200">Rating found</response>
    /// <response code="404">Rating not found</response>
    /// <response code="401">User not authenticated</response>
    [HttpGet("rating/movie/{movieId:guid}")]
    [Authorize]
    [SwaggerResponseExample(200, typeof(Swagger.Examples.RatingDtoExample))]
    [ProducesResponseType(typeof(RatingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUserRating(Guid movieId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Geçersiz token." });
        }

        var query = new GetUserRatingQuery(userId, movieId);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Error });
        }

        if (result.Data == null)
        {
            return NotFound(new { message = "Bu film için puanınız bulunamadı." });
        }

        var response = new RatingDto
        {
            Id = result.Data.Id,
            UserId = result.Data.UserId,
            MovieId = result.Data.MovieId,
            Score = result.Data.Score,
            CreatedAt = result.Data.CreatedAt,
            UpdatedAt = result.Data.UpdatedAt
        };

        return Ok(response);
    }

    /// <summary>
    /// Get movie rating statistics
    /// </summary>
    /// <param name="movieId">Movie ID</param>
    /// <returns>Rating statistics for the movie</returns>
    /// <response code="200">Rating stats retrieved successfully</response>
    /// <response code="404">Movie not found</response>
    [HttpGet("rating/movie/{movieId:guid}/stats")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMovieRatingStats(Guid movieId)
    {
        var query = new GetMovieRatingsQuery(movieId);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Error });
        }

        var response = new
        {
            MovieId = result.Data!.MovieId,
            AverageRating = result.Data.AverageRating,
            TotalRatings = result.Data.TotalRatings,
            RatingDistribution = result.Data.RatingDistribution
        };

        return Ok(response);
    }

    /// <summary>
    /// Delete user's rating for a movie
    /// </summary>
    /// <param name="movieId">Movie ID</param>
    /// <returns>Confirmation of deletion</returns>
    /// <response code="204">Rating deleted successfully</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="404">Rating not found</response>
    [HttpDelete("rating/movie/{movieId:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRating(Guid movieId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Geçersiz token." });
        }

        var command = new DeleteRatingCommand(userId, movieId);
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Error });
        }

        return NoContent();
    }
}