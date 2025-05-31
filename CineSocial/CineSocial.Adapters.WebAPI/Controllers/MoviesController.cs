using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CineSocial.Core.Application.Ports;
using CineSocial.Core.Application.DTOs.Movies;
using CineSocial.Adapters.WebAPI.DTOs.Responses;

namespace CineSocial.Adapters.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _movieService;
    private readonly ILogger<MoviesController> _logger;

    public MoviesController(IMovieService movieService, ILogger<MoviesController> logger)
    {
        _movieService = movieService;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetMovies(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] List<Guid>? genreIds = null,
        [FromQuery] string? sortBy = null)
    {
        try
        {
            var result = await _movieService.GetMoviesAsync(page, pageSize, search, genreIds, sortBy);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse.CreateFailure(result.ErrorMessage));
            }

            return Ok(ApiResponse<PaginatedResponse<MovieSummaryDto>>.CreateSuccess(
                new PaginatedResponse<MovieSummaryDto>
                {
                    Items = result.Value!.Items,
                    TotalCount = result.Value.TotalCount,
                    Page = result.Value.Page,
                    PageSize = result.Value.PageSize
                }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetMovies endpoint error");
            return StatusCode(500, ApiResponse.CreateFailure("Bir hata oluştu"));
        }
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetMovie(Guid id)
    {
        try
        {
            var result = await _movieService.GetMovieByIdAsync(id);

            if (!result.IsSuccess)
            {
                return NotFound(ApiResponse.CreateFailure(result.ErrorMessage));
            }

            return Ok(ApiResponse<MovieDto>.CreateSuccess(result.Value!));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetMovie endpoint error");
            return StatusCode(500, ApiResponse.CreateFailure("Bir hata oluştu"));
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateMovie([FromBody] CreateMovieDto createDto)
    {
        try
        {
            var result = await _movieService.CreateMovieAsync(createDto);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse.CreateFailure(result.ErrorMessage));
            }

            return CreatedAtAction(nameof(GetMovie), new { id = result.Value!.Id }, 
                ApiResponse<MovieDto>.CreateSuccess(result.Value, "Film başarıyla oluşturuldu"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateMovie endpoint error");
            return StatusCode(500, ApiResponse.CreateFailure("Bir hata oluştu"));
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateMovie(Guid id, [FromBody] UpdateMovieDto updateDto)
    {
        try
        {
            var result = await _movieService.UpdateMovieAsync(id, updateDto);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse.CreateFailure(result.ErrorMessage));
            }

            return Ok(ApiResponse<MovieDto>.CreateSuccess(result.Value!, "Film başarıyla güncellendi"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateMovie endpoint error");
            return StatusCode(500, ApiResponse.CreateFailure("Bir hata oluştu"));
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteMovie(Guid id)
    {
        try
        {
            var result = await _movieService.DeleteMovieAsync(id);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse.CreateFailure(result.ErrorMessage));
            }

            return Ok(ApiResponse.CreateSuccess("Film başarıyla silindi"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DeleteMovie endpoint error");
            return StatusCode(500, ApiResponse.CreateFailure("Bir hata oluştu"));
        }
    }

    [HttpGet("popular")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPopularMovies([FromQuery] int count = 10)
    {
        try
        {
            var result = await _movieService.GetPopularMoviesAsync(count);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse.CreateFailure(result.ErrorMessage));
            }

            return Ok(ApiResponse<List<MovieSummaryDto>>.CreateSuccess(result.Value!));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetPopularMovies endpoint error");
            return StatusCode(500, ApiResponse.CreateFailure("Bir hata oluştu"));
        }
    }

    [HttpGet("top-rated")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTopRatedMovies([FromQuery] int count = 10)
    {
        try
        {
            var result = await _movieService.GetTopRatedMoviesAsync(count);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse.CreateFailure(result.ErrorMessage));
            }

            return Ok(ApiResponse<List<MovieSummaryDto>>.CreateSuccess(result.Value!));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetTopRatedMovies endpoint error");
            return StatusCode(500, ApiResponse.CreateFailure("Bir hata oluştu"));
        }
    }

    [HttpGet("recent")]
    [AllowAnonymous]
    public async Task<IActionResult> GetRecentMovies([FromQuery] int count = 10)
    {
        try
        {
            var result = await _movieService.GetRecentMoviesAsync(count);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse.CreateFailure(result.ErrorMessage));
            }

            return Ok(ApiResponse<List<MovieSummaryDto>>.CreateSuccess(result.Value!));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetRecentMovies endpoint error");
            return StatusCode(500, ApiResponse.CreateFailure("Bir hata oluştu"));
        }
    }
}
