using CineSocial.Application.Features.Lists.Commands.CreateMovieList;
using CineSocial.Application.Features.Lists.Commands.UpdateMovieList;
using CineSocial.Application.Features.Lists.Commands.DeleteMovieList;
using CineSocial.Application.Features.Lists.Commands.AddMovieToList;
using CineSocial.Application.Features.Lists.Commands.RemoveMovieFromList;
using CineSocial.Application.Features.Lists.Commands.ReorderMovieInList;
using CineSocial.Application.Features.Lists.Commands.FavoriteMovieList;
using CineSocial.Application.Features.Lists.Commands.UnfavoriteMovieList;
using CineSocial.Application.Features.Lists.Queries.GetPublicMovieLists;
using CineSocial.Application.Features.Lists.Queries.GetMovieListById;
using CineSocial.Application.Features.Lists.Queries.GetUserMovieLists;
using CineSocial.Application.Features.Lists.Queries.GetUserWatchlist;
using CineSocial.Application.Features.Lists.Queries.GetUserFavoriteLists;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineSocial.Api.Controllers;

/// <summary>
/// Movie list management endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ListController : ControllerBase
{
    private readonly IMediator _mediator;

    public ListController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all public movie lists (sorted by popularity)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPublicLists(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetPublicMovieListsQuery(page, pageSize);
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(new { data = result.Data, page, pageSize });
    }

    /// <summary>
    /// Get a movie list by ID with all its items
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetList(int id, CancellationToken cancellationToken = default)
    {
        var query = new GetMovieListByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result.Data);
    }

    /// <summary>
    /// Get current user's movie lists
    /// </summary>
    [HttpGet("my")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyLists(CancellationToken cancellationToken = default)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var query = new GetUserMovieListsQuery(userId);
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result.Data);
    }

    /// <summary>
    /// Get current user's watchlist
    /// </summary>
    [HttpGet("my/watchlist")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyWatchlist(CancellationToken cancellationToken = default)
    {
        var query = new GetUserWatchlistQuery();
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result.Data);
    }

    /// <summary>
    /// Get lists favorited by current user
    /// </summary>
    [HttpGet("my/favorites")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyFavorites(CancellationToken cancellationToken = default)
    {
        var query = new GetUserFavoriteListsQuery();
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result.Data);
    }

    /// <summary>
    /// Get a user's public movie lists
    /// </summary>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserLists(int userId, CancellationToken cancellationToken = default)
    {
        var query = new GetUserMovieListsQuery(userId);
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result.Data);
    }

    /// <summary>
    /// Create a new movie list
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateList(
        [FromBody] CreateListRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateMovieListCommand(
            request.Name,
            request.Description,
            request.IsPublic,
            request.CoverImageId);

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetList), new { id = result.Data!.Id }, result.Data);
    }

    /// <summary>
    /// Update a movie list
    /// </summary>
    [HttpPut("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateList(
        int id,
        [FromBody] UpdateListRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateMovieListCommand(
            id,
            request.Name,
            request.Description,
            request.IsPublic,
            request.CoverImageId);

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(new { message = "List updated successfully" });
    }

    /// <summary>
    /// Delete a movie list
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteList(int id, CancellationToken cancellationToken = default)
    {
        var command = new DeleteMovieListCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return NoContent();
    }

    /// <summary>
    /// Add a movie to a list
    /// </summary>
    [HttpPost("{id}/movies")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddMovie(
        int id,
        [FromBody] AddMovieRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new AddMovieToListCommand(id, request.MovieId);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(new { message = "Movie added successfully" });
    }

    /// <summary>
    /// Remove a movie from a list
    /// </summary>
    [HttpDelete("{id}/movies/{movieId}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RemoveMovie(
        int id,
        int movieId,
        CancellationToken cancellationToken = default)
    {
        var command = new RemoveMovieFromListCommand(id, movieId);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return NoContent();
    }

    /// <summary>
    /// Reorder a movie in a list
    /// </summary>
    [HttpPut("{id}/movies/{movieId}/order")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ReorderMovie(
        int id,
        int movieId,
        [FromBody] ReorderMovieRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new ReorderMovieInListCommand(id, movieId, request.NewOrder);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(new { message = "Movie reordered successfully" });
    }

    /// <summary>
    /// Favorite a movie list
    /// </summary>
    [HttpPost("{id}/favorite")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> FavoriteList(int id, CancellationToken cancellationToken = default)
    {
        var command = new FavoriteMovieListCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(new { message = "List favorited successfully" });
    }

    /// <summary>
    /// Unfavorite a movie list
    /// </summary>
    [HttpDelete("{id}/favorite")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UnfavoriteList(int id, CancellationToken cancellationToken = default)
    {
        var command = new UnfavoriteMovieListCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return NoContent();
    }
}

// Request DTOs
public record CreateListRequest(string Name, string? Description, bool IsPublic, int? CoverImageId);
public record UpdateListRequest(string? Name, string? Description, bool? IsPublic, int? CoverImageId);
public record AddMovieRequest(int MovieId);
public record ReorderMovieRequest(int NewOrder);
