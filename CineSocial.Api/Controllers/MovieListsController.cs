using CineSocial.Api.DTOs;
using CineSocial.Api.Extensions;
using CineSocial.Core.Features.MovieLists.Commands;
using CineSocial.Core.Features.MovieLists.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CineSocial.Api.Controllers;

/// <summary>
/// Movie Lists API endpoints for managing user movie lists, watchlists, and list favorites
/// </summary>
[ApiController]
[Route("api/movie-lists")]
public class MovieListsController : ControllerBase
{
    private readonly IMediator _mediator;

    public MovieListsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get public movie lists with pagination and search
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Number of items per page (default: 10, max: 50)</param>
    /// <param name="search">Search term for list name or description</param>
    /// <returns>Paginated list of public movie lists</returns>
    /// <response code="200">Returns paginated public movie lists</response>
    /// <response code="400">Invalid pagination parameters</response>
    [HttpGet("public")]
    public async Task<IActionResult> GetPublicMovieLists(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null)
    {
        var query = new GetPublicMovieListsQuery(page, pageSize, search);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        var dto = new PagedUserMovieListDto
        {
            MovieLists = result.Data!.MovieLists.Select(ml => new UserMovieListDto
            {
                Id = ml.Id,
                UserId = ml.UserId,
                Name = ml.Name,
                Description = ml.Description,
                IsPublic = ml.IsPublic,
                MovieCount = ml.MovieCount,
                CreatedAt = ml.CreatedAt,
                UpdatedAt = ml.UpdatedAt,
                UserFullName = ml.UserFullName,
                UserUsername = ml.UserUsername
            }).ToList(),
            TotalCount = result.Data.TotalCount,
            Page = result.Data.Page,
            PageSize = result.Data.PageSize,
            TotalPages = result.Data.TotalPages
        };

        return Ok(dto);
    }

    /// <summary>
    /// Get movie list details by ID
    /// </summary>
    /// <param name="id">Movie list ID</param>
    /// <returns>Detailed movie list with all movies</returns>
    /// <response code="200">Returns movie list details</response>
    /// <response code="404">Movie list not found</response>
    /// <response code="403">Access denied to private list</response>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetMovieListById(Guid id)
    {
        var userId = GetCurrentUserId();
        var query = new GetMovieListByIdQuery(id, userId);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return NotFound(result.Error);

        var dto = new MovieListDetailDto
        {
            Id = result.Data!.Id,
            UserId = result.Data.UserId,
            Name = result.Data.Name,
            Description = result.Data.Description,
            IsPublic = result.Data.IsPublic,
            IsWatchlist = result.Data.IsWatchlist,
            Movies = result.Data.Movies.Select(m => new MovieListItemDto
            {
                Id = m.Id,
                MovieId = m.MovieId,
                MovieTitle = m.MovieTitle,
                MoviePosterPath = m.MoviePosterPath,
                Notes = m.Notes,
                Order = m.Order,
                AddedAt = m.AddedAt
            }).ToList(),
            CreatedAt = result.Data.CreatedAt,
            UpdatedAt = result.Data.UpdatedAt,
            UserFullName = result.Data.UserFullName,
            UserUsername = result.Data.UserUsername,
            IsFavorited = result.Data.IsFavorited
        };

        return Ok(dto);
    }

    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetUserMovieLists(Guid userId, [FromQuery] bool includeWatchlist = true)
    {
        var query = new GetUserMovieListsQuery(userId, includeWatchlist);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        var dtos = result.Data!.Select(ml => new UserMovieListDto
        {
            Id = ml.Id,
            UserId = ml.UserId,
            Name = ml.Name,
            Description = ml.Description,
            IsPublic = ml.IsPublic,
            IsWatchlist = ml.IsWatchlist,
            MovieCount = ml.MovieCount,
            CreatedAt = ml.CreatedAt,
            UpdatedAt = ml.UpdatedAt,
            UserFullName = ml.UserFullName,
            UserUsername = ml.UserUsername
        }).ToList();

        return Ok(dtos);
    }

    [HttpGet("favorites")]
    [Authorize]
    public async Task<IActionResult> GetUserFavoriteLists(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var userId = GetCurrentUserId()!.Value;
        var query = new GetUserFavoriteListsQuery(userId, page, pageSize);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        var dto = new PagedUserMovieListDto
        {
            MovieLists = result.Data!.MovieLists.Select(ml => new UserMovieListDto
            {
                Id = ml.Id,
                UserId = ml.UserId,
                Name = ml.Name,
                Description = ml.Description,
                IsPublic = ml.IsPublic,
                MovieCount = ml.MovieCount,
                CreatedAt = ml.CreatedAt,
                UpdatedAt = ml.UpdatedAt,
                UserFullName = ml.UserFullName,
                UserUsername = ml.UserUsername,
                IsFavorited = true
            }).ToList(),
            TotalCount = result.Data.TotalCount,
            Page = result.Data.Page,
            PageSize = result.Data.PageSize,
            TotalPages = result.Data.TotalPages
        };

        return Ok(dto);
    }

    /// <summary>
    /// Create a new movie list
    /// </summary>
    /// <param name="request">Movie list creation data</param>
    /// <returns>Created movie list</returns>
    /// <response code="201">Movie list created successfully</response>
    /// <response code="400">Invalid request data or duplicate list name</response>
    /// <response code="401">User not authenticated</response>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateMovieList([FromBody] CreateMovieListRequest request)
    {
        var userId = GetCurrentUserId()!.Value;
        var command = new CreateMovieListCommand(userId, request.Name, request.Description, request.IsPublic);
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        var dto = new UserMovieListDto
        {
            Id = result.Data!.Id,
            UserId = result.Data.UserId,
            Name = result.Data.Name,
            Description = result.Data.Description,
            IsPublic = result.Data.IsPublic,
            MovieCount = result.Data.MovieCount,
            CreatedAt = result.Data.CreatedAt,
            UpdatedAt = result.Data.UpdatedAt,
            UserFullName = result.Data.UserFullName,
            UserUsername = result.Data.UserUsername
        };

        return CreatedAtAction(nameof(GetMovieListById), new { id = dto.Id }, dto);
    }

    [HttpPost("watchlist")]
    [Authorize]
    public async Task<IActionResult> CreateWatchlist()
    {
        var userId = GetCurrentUserId()!.Value;
        var command = new CreateWatchlistCommand(userId);
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        var dto = new UserMovieListDto
        {
            Id = result.Data!.Id,
            UserId = result.Data.UserId,
            Name = result.Data.Name,
            Description = result.Data.Description,
            IsPublic = result.Data.IsPublic,
            IsWatchlist = true,
            MovieCount = result.Data.MovieCount,
            CreatedAt = result.Data.CreatedAt,
            UpdatedAt = result.Data.UpdatedAt,
            UserFullName = result.Data.UserFullName,
            UserUsername = result.Data.UserUsername
        };

        return CreatedAtAction(nameof(GetMovieListById), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateMovieList(Guid id, [FromBody] UpdateMovieListRequest request)
    {
        var userId = GetCurrentUserId()!.Value;
        var command = new UpdateMovieListCommand(id, userId, request.Name, request.Description, request.IsPublic);
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        var dto = new UserMovieListDto
        {
            Id = result.Data!.Id,
            UserId = result.Data.UserId,
            Name = result.Data.Name,
            Description = result.Data.Description,
            IsPublic = result.Data.IsPublic,
            MovieCount = result.Data.MovieCount,
            CreatedAt = result.Data.CreatedAt,
            UpdatedAt = result.Data.UpdatedAt,
            UserFullName = result.Data.UserFullName,
            UserUsername = result.Data.UserUsername
        };

        return Ok(dto);
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteMovieList(Guid id)
    {
        var userId = GetCurrentUserId()!.Value;
        var command = new DeleteMovieListCommand(id, userId);
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return NoContent();
    }

    /// <summary>
    /// Add a movie to a movie list
    /// </summary>
    /// <param name="listId">Movie list ID</param>
    /// <param name="request">Movie addition data</param>
    /// <returns>Added movie list item</returns>
    /// <response code="201">Movie added to list successfully</response>
    /// <response code="400">Movie already in list or invalid data</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="403">User doesn't own the list</response>
    /// <response code="404">List or movie not found</response>
    [HttpPost("{listId:guid}/movies")]
    [Authorize]
    public async Task<IActionResult> AddMovieToList(Guid listId, [FromBody] AddMovieToListRequest request)
    {
        var userId = GetCurrentUserId()!.Value;
        var command = new AddMovieToListCommand(userId, listId, request.MovieId, request.Notes);
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        var dto = new MovieListItemDto
        {
            Id = result.Data!.Id,
            MovieId = result.Data.MovieId,
            MovieTitle = result.Data.MovieTitle,
            MoviePosterPath = result.Data.MoviePosterPath,
            Notes = result.Data.Notes,
            Order = result.Data.Order,
            AddedAt = result.Data.CreatedAt
        };

        return Created($"/api/movie-lists/{listId}/movies/{dto.Id}", dto);
    }

    [HttpDelete("{listId:guid}/movies/{movieId:guid}")]
    [Authorize]
    public async Task<IActionResult> RemoveMovieFromList(Guid listId, Guid movieId)
    {
        var userId = GetCurrentUserId()!.Value;
        var command = new RemoveMovieFromListCommand(userId, listId, movieId);
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return NoContent();
    }

    [HttpPost("{listId:guid}/favorite")]
    [Authorize]
    public async Task<IActionResult> AddListToFavorites(Guid listId)
    {
        var userId = GetCurrentUserId()!.Value;
        var command = new AddListToFavoritesCommand(userId, listId);
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok();
    }

    [HttpDelete("{listId:guid}/favorite")]
    [Authorize]
    public async Task<IActionResult> RemoveListFromFavorites(Guid listId)
    {
        var userId = GetCurrentUserId()!.Value;
        var command = new RemoveListFromFavoritesCommand(userId, listId);
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return NoContent();
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}