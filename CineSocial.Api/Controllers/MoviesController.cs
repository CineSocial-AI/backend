using CineSocial.Api.DTOs;
using CineSocial.Core.Features.Movies.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace CineSocial.Api.Controllers;

/// <summary>
/// Movie information and search endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class MoviesController : ControllerBase
{
    private readonly IMediator _mediator;

    public MoviesController(IMediator mediator)
    {
        _mediator = mediator;
    }
    /// <summary>
    /// Get all movies with optional filtering and pagination
    /// </summary>
    /// <param name="query">Search query for movie title</param>
    /// <param name="genreIds">Filter by genre IDs</param>
    /// <param name="year">Filter by release year</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Items per page (default: 10, max: 50)</param>
    /// <param name="sortBy">Sort field (popularity, title, release_date, vote_average)</param>
    /// <param name="sortOrder">Sort order (asc, desc)</param>
    /// <returns>Paginated list of movies</returns>
    /// <response code="200">Movies retrieved successfully</response>
    /// <response code="400">Invalid parameters</response>
    [HttpGet]
    [SwaggerResponseExample(200, typeof(Swagger.Examples.MovieListDtoExample))]
    [ProducesResponseType(typeof(MovieListDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetMovies(
        [FromQuery] string? query = null,
        [FromQuery] List<Guid>? genreIds = null,
        [FromQuery] int? year = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string sortBy = "popularity",
        [FromQuery] string sortOrder = "desc")
    {
        var getMoviesQuery = new GetMoviesQuery(query, genreIds, year, page, pageSize, sortBy, sortOrder);
        var result = await _mediator.Send(getMoviesQuery);

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Error });
        }

        var response = new MovieListDto
        {
            Movies = result.Data!.Movies.Select(m => new MovieDto
            {
                Id = m.Id,
                Title = m.Title,
                OriginalTitle = m.OriginalTitle,
                Overview = m.Overview,
                ReleaseDate = m.ReleaseDate,
                Runtime = m.Runtime,
                VoteAverage = m.VoteAverage,
                VoteCount = m.VoteCount,
                Language = m.Language,
                Popularity = m.Popularity,
                Status = m.Status,
                Budget = m.Budget,
                Revenue = m.Revenue,
                Tagline = m.Tagline,
                Genres = m.Genres.Select(g => new GenreDto
                {
                    Id = g.Id,
                    Name = g.Name,
                    Description = g.Description
                }).ToList()
            }).ToList(),
            TotalCount = result.Data.TotalCount,
            Page = result.Data.Page,
            PageSize = result.Data.PageSize,
            TotalPages = result.Data.TotalPages
        };

        return Ok(response);
    }

    /// <summary>
    /// Search movies with advanced filtering
    /// </summary>
    /// <param name="request">Search criteria</param>
    /// <returns>Paginated search results</returns>
    /// <response code="200">Search completed successfully</response>
    /// <response code="400">Invalid search parameters</response>
    [HttpPost("search")]
    [SwaggerRequestExample(typeof(MovieSearchRequest), typeof(Swagger.Examples.MovieSearchRequestExample))]
    [SwaggerResponseExample(200, typeof(Swagger.Examples.MovieListDtoExample))]
    [ProducesResponseType(typeof(MovieListDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchMovies([FromBody] MovieSearchRequest request)
    {
        var query = new GetMoviesQuery(
            request.Query,
            request.GenreIds,
            request.Year,
            request.Page,
            request.PageSize,
            request.SortBy ?? "popularity",
            request.SortOrder ?? "desc"
        );
        
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Error });
        }

        var response = new MovieListDto
        {
            Movies = result.Data!.Movies.Select(m => new MovieDto
            {
                Id = m.Id,
                Title = m.Title,
                OriginalTitle = m.OriginalTitle,
                Overview = m.Overview,
                ReleaseDate = m.ReleaseDate,
                Runtime = m.Runtime,
                VoteAverage = m.VoteAverage,
                VoteCount = m.VoteCount,
                Language = m.Language,
                Popularity = m.Popularity,
                Status = m.Status,
                Budget = m.Budget,
                Revenue = m.Revenue,
                Tagline = m.Tagline,
                Genres = m.Genres.Select(g => new GenreDto
                {
                    Id = g.Id,
                    Name = g.Name,
                    Description = g.Description
                }).ToList()
            }).ToList(),
            TotalCount = result.Data.TotalCount,
            Page = result.Data.Page,
            PageSize = result.Data.PageSize,
            TotalPages = result.Data.TotalPages
        };

        return Ok(response);
    }

    /// <summary>
    /// Get movie details by ID
    /// </summary>
    /// <param name="id">Movie ID</param>
    /// <returns>Detailed movie information including cast and crew</returns>
    /// <response code="200">Movie found</response>
    /// <response code="404">Movie not found</response>
    [HttpGet("{id:guid}")]
    [SwaggerResponseExample(200, typeof(Swagger.Examples.MovieDtoExample))]
    [ProducesResponseType(typeof(MovieDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMovie(Guid id)
    {
        var query = new GetMovieByIdQuery(id);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
        {
            return NotFound(new { message = result.Error });
        }

        var movie = result.Data!;
        var response = new MovieDto
        {
            Id = movie.Id,
            Title = movie.Title,
            OriginalTitle = movie.OriginalTitle,
            Overview = movie.Overview,
            ReleaseDate = movie.ReleaseDate,
            Runtime = movie.Runtime,
            VoteAverage = movie.VoteAverage,
            VoteCount = movie.VoteCount,
            Language = movie.Language,
            Popularity = movie.Popularity,
            Status = movie.Status,
            Budget = movie.Budget,
            Revenue = movie.Revenue,
            Tagline = movie.Tagline,
            Genres = movie.Genres.Select(g => new GenreDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description
            }).ToList(),
            Cast = movie.Cast.Select(c => new MovieCastDto
            {
                PersonId = c.PersonId,
                Name = c.Name,
                Character = c.Character,
                Order = c.Order
            }).ToList(),
            Crew = movie.Crew.Select(c => new MovieCrewDto
            {
                PersonId = c.PersonId,
                Name = c.Name,
                Job = c.Job,
                Department = c.Department
            }).ToList()
        };

        return Ok(response);
    }

    /// <summary>
    /// Get movie genres
    /// </summary>
    /// <returns>List of available genres</returns>
    /// <response code="200">Genres retrieved successfully</response>
    [HttpGet("genres")]
    [SwaggerResponseExample(200, typeof(Swagger.Examples.GenreDtoExample))]
    [ProducesResponseType(typeof(List<GenreDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetGenres()
    {
        var query = new GetGenresQuery();
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Error });
        }

        var response = result.Data!.Select(g => new GenreDto
        {
            Id = g.Id,
            Name = g.Name,
            Description = g.Description
        }).ToList();

        return Ok(response);
    }

    /// <summary>
    /// Get popular movies
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Items per page (default: 20, max: 50)</param>
    /// <returns>Popular movies list</returns>
    /// <response code="200">Popular movies retrieved successfully</response>
    [HttpGet("popular")]
    [SwaggerResponseExample(200, typeof(Swagger.Examples.MovieListDtoExample))]
    [ProducesResponseType(typeof(MovieListDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPopularMovies([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var query = new GetMoviesQuery(
            Query: null,
            GenreIds: null,
            Year: null,
            Page: page,
            PageSize: pageSize,
            SortBy: "popularity",
            SortOrder: "desc"
        );
        
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Error });
        }

        var response = new MovieListDto
        {
            Movies = result.Data!.Movies.Select(m => new MovieDto
            {
                Id = m.Id,
                Title = m.Title,
                OriginalTitle = m.OriginalTitle,
                Overview = m.Overview,
                ReleaseDate = m.ReleaseDate,
                Runtime = m.Runtime,
                VoteAverage = m.VoteAverage,
                VoteCount = m.VoteCount,
                Language = m.Language,
                Popularity = m.Popularity,
                Status = m.Status,
                Budget = m.Budget,
                Revenue = m.Revenue,
                Tagline = m.Tagline,
                Genres = m.Genres.Select(g => new GenreDto
                {
                    Id = g.Id,
                    Name = g.Name,
                    Description = g.Description
                }).ToList()
            }).ToList(),
            TotalCount = result.Data.TotalCount,
            Page = result.Data.Page,
            PageSize = result.Data.PageSize,
            TotalPages = result.Data.TotalPages
        };

        return Ok(response);
    }

    /// <summary>
    /// Get top rated movies
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Items per page (default: 20, max: 50)</param>
    /// <returns>Top rated movies list</returns>
    /// <response code="200">Top rated movies retrieved successfully</response>
    [HttpGet("top-rated")]
    [SwaggerResponseExample(200, typeof(Swagger.Examples.MovieListDtoExample))]
    [ProducesResponseType(typeof(MovieListDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTopRatedMovies([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var query = new GetMoviesQuery(
            Query: null,
            GenreIds: null,
            Year: null,
            Page: page,
            PageSize: pageSize,
            SortBy: "vote_average",
            SortOrder: "desc"
        );
        
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Error });
        }

        var response = new MovieListDto
        {
            Movies = result.Data!.Movies.Select(m => new MovieDto
            {
                Id = m.Id,
                Title = m.Title,
                OriginalTitle = m.OriginalTitle,
                Overview = m.Overview,
                ReleaseDate = m.ReleaseDate,
                Runtime = m.Runtime,
                VoteAverage = m.VoteAverage,
                VoteCount = m.VoteCount,
                Language = m.Language,
                Popularity = m.Popularity,
                Status = m.Status,
                Budget = m.Budget,
                Revenue = m.Revenue,
                Tagline = m.Tagline,
                Genres = m.Genres.Select(g => new GenreDto
                {
                    Id = g.Id,
                    Name = g.Name,
                    Description = g.Description
                }).ToList()
            }).ToList(),
            TotalCount = result.Data.TotalCount,
            Page = result.Data.Page,
            PageSize = result.Data.PageSize,
            TotalPages = result.Data.TotalPages
        };

        return Ok(response);
    }

    /// <summary>
    /// Get upcoming movies
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Items per page (default: 20, max: 50)</param>
    /// <returns>Upcoming movies list</returns>
    /// <response code="200">Upcoming movies retrieved successfully</response>
    [HttpGet("upcoming")]
    [SwaggerResponseExample(200, typeof(Swagger.Examples.MovieListDtoExample))]
    [ProducesResponseType(typeof(MovieListDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUpcomingMovies([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var currentDate = DateTime.UtcNow;
        var query = new GetMoviesQuery(
            Query: null,
            GenreIds: null,
            Year: null,
            Page: page,
            PageSize: pageSize,
            SortBy: "release_date",
            SortOrder: "asc",
            IsUpcoming: true
        );
        
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Error });
        }

        var response = new MovieListDto
        {
            Movies = result.Data!.Movies
                .Where(m => m.ReleaseDate > currentDate) // Future releases only
                .Select(m => new MovieDto
                {
                    Id = m.Id,
                    Title = m.Title,
                    OriginalTitle = m.OriginalTitle,
                    Overview = m.Overview,
                    ReleaseDate = m.ReleaseDate,
                    Runtime = m.Runtime,
                    VoteAverage = m.VoteAverage,
                    VoteCount = m.VoteCount,
                    Language = m.Language,
                    Popularity = m.Popularity,
                    Status = m.Status,
                    Budget = m.Budget,
                    Revenue = m.Revenue,
                    Tagline = m.Tagline,
                    Genres = m.Genres.Select(g => new GenreDto
                    {
                        Id = g.Id,
                        Name = g.Name,
                        Description = g.Description
                    }).ToList()
                }).ToList(),
            TotalCount = result.Data.TotalCount,
            Page = result.Data.Page,
            PageSize = result.Data.PageSize,
            TotalPages = result.Data.TotalPages
        };

        return Ok(response);
    }
}