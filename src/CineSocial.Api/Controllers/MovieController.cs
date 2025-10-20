using CineSocial.Application.Features.Movies.Queries.GetAll;
using CineSocial.Application.Features.Movies.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CineSocial.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
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
    public async Task<IActionResult> GetById(int id)
    {
        var query = new GetMovieByIdQuery(id);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }
}
