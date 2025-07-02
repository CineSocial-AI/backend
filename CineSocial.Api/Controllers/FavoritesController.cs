using System.Security.Claims;
using CineSocial.Api.DTOs;
using CineSocial.Core.Features.Favorites.Commands;
using CineSocial.Core.Features.Favorites.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace CineSocial.Api.Controllers;

/// <summary>
/// User favorites management endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class FavoritesController : ControllerBase
{
    private readonly IMediator _mediator;

    public FavoritesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get user's favorite movies
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Items per page (default: 10, max: 50)</param>
    /// <param name="sortBy">Sort field (created_at, movie_title, movie_release_date)</param>
    /// <param name="sortOrder">Sort order (asc, desc)</param>
    /// <returns>Paginated list of user's favorite movies</returns>
    /// <response code="200">Favorites retrieved successfully</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="404">User not found</response>
    [HttpGet]
    [SwaggerResponseExample(200, typeof(Swagger.Examples.FavoriteDtoExample))]
    [ProducesResponseType(typeof(List<FavoriteDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserFavorites(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string sortBy = "created_at",
        [FromQuery] string sortOrder = "desc")
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Geçersiz token." });
        }

        pageSize = Math.Min(pageSize, 50);

        var query = new GetUserFavoritesQuery(userId, page, pageSize, sortBy, sortOrder);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Error });
        }

        var response = result.Data!.Favorites.Select(f => new FavoriteDto
        {
            Id = f.Id,
            UserId = f.UserId,
            MovieId = f.MovieId,
            MovieTitle = f.MovieTitle,
            MoviePosterPath = f.MoviePosterPath,
            CreatedAt = f.CreatedAt
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
    /// Add a movie to favorites
    /// </summary>
    /// <param name="request">Favorite information</param>
    /// <returns>Created favorite</returns>
    /// <response code="201">Movie added to favorites successfully</response>
    /// <response code="400">Invalid data or movie already in favorites</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="404">Movie not found</response>
    [HttpPost]
    [SwaggerRequestExample(typeof(AddToFavoritesRequest), typeof(Swagger.Examples.AddToFavoritesRequestExample))]
    [SwaggerResponseExample(201, typeof(Swagger.Examples.FavoriteDtoExample))]
    [ProducesResponseType(typeof(FavoriteDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddToFavorites([FromBody] AddToFavoritesRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Geçersiz token." });
        }

        var command = new AddToFavoritesCommand(userId, request.MovieId);
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Error });
        }

        var response = new FavoriteDto
        {
            Id = result.Data!.Id,
            UserId = result.Data.UserId,
            MovieId = result.Data.MovieId,
            MovieTitle = result.Data.MovieTitle,
            MoviePosterPath = result.Data.MoviePosterPath,
            CreatedAt = result.Data.CreatedAt
        };

        return CreatedAtAction(nameof(CheckIsFavorite), new { movieId = response.MovieId }, response);
    }

    /// <summary>
    /// Remove a movie from favorites
    /// </summary>
    /// <param name="movieId">Movie ID</param>
    /// <returns>Confirmation of removal</returns>
    /// <response code="204">Movie removed from favorites successfully</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="404">Movie not found in favorites</response>
    [HttpDelete("{movieId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveFromFavorites(Guid movieId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Geçersiz token." });
        }

        var command = new RemoveFromFavoritesCommand(userId, movieId);
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Error });
        }

        return NoContent();
    }

    /// <summary>
    /// Check if a movie is in user's favorites
    /// </summary>
    /// <param name="movieId">Movie ID</param>
    /// <returns>Boolean indicating if movie is favorited</returns>
    /// <response code="200">Check completed successfully</response>
    /// <response code="401">User not authenticated</response>
    [HttpGet("check/{movieId:guid}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CheckIsFavorite(Guid movieId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Geçersiz token." });
        }

        var query = new CheckIsFavoriteQuery(userId, movieId);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Error });
        }

        var response = new
        {
            MovieId = movieId,
            IsFavorite = result.Data
        };

        return Ok(response);
    }
}