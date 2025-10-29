using CineSocial.Application.Features.Movies.Queries.GetAll;
using CineSocial.Application.Features.Movies.Queries.GetById;
using CineSocial.Application.Features.Movies.Queries.GetPopular;
using CineSocial.Application.Features.Movies.Queries.GetTopRated;
using CineSocial.Application.Features.Movies.Queries.GetByYear;
using CineSocial.Application.Features.Movies.Queries.GetByPerson;
using CineSocial.Application.Features.Movies.Queries.GetNewReleases;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CineSocial.Api.Controllers;

/// <summary>
/// Movie browsing and discovery endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class MovieController : ControllerBase
{
    private readonly IMediator _mediator;

    public MovieController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all movies with pagination, search and sorting
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortBy = "Popularity",
        [FromQuery] bool sortDescending = true)
    {
        var query = new GetAllMoviesQuery
        {
            Page = page,
            PageSize = pageSize,
            SearchTerm = searchTerm,
            SortBy = sortBy,
            SortDescending = sortDescending
        };

        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Get movie by ID with full details
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new GetMovieByIdQuery(id);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    /// <summary>
    /// Get popular movies (sorted by popularity DESC)
    /// </summary>
    [HttpGet("popular")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPopular(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetPopularMoviesQuery(page, pageSize);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result.Data);
    }

    /// <summary>
    /// Get top rated movies (sorted by vote average DESC)
    /// </summary>
    [HttpGet("top-rated")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTopRated(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] int minVotes = 100)
    {
        var query = new GetTopRatedMoviesQuery(page, pageSize, minVotes);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result.Data);
    }

    /// <summary>
    /// Get movies by genre
    /// </summary>
    [HttpGet("genre/{genreId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByGenre(
        int genreId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new CineSocial.Application.Features.Genres.Queries.GetMovies.GetGenreMoviesQuery(genreId, page, pageSize);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result.Data);
    }

    /// <summary>
    /// Get movies by release year
    /// </summary>
    [HttpGet("year/{year}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByYear(
        int year,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetMoviesByYearQuery(year, page, pageSize);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result.Data);
    }

    /// <summary>
    /// Get movies by person (actor/director/crew)
    /// </summary>
    [HttpGet("person/{personId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByPerson(
        int personId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetMoviesByPersonQuery(personId, page, pageSize);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result.Data);
    }

    /// <summary>
    /// Get new releases (movies released in last 90 days)
    /// </summary>
    [HttpGet("new-releases")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetNewReleases(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] int days = 90)
    {
        var query = new GetNewReleasesQuery(page, pageSize, days);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result.Data);
    }
}
