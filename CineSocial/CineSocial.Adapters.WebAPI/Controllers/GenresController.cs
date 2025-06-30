using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CineSocial.Core.Application.Services;
using CineSocial.Core.Application.DTOs.Movies;
using CineSocial.Adapters.WebAPI.DTOs.Responses;

namespace CineSocial.Adapters.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class GenresController : ControllerBase
{
    private readonly IMovieService _movieService;
    private readonly ILogger<GenresController> _logger;

    public GenresController(IMovieService movieService, ILogger<GenresController> logger)
    {
        _movieService = movieService;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetGenres(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _movieService.GetGenresAsync(cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse.CreateFailure(result.ErrorMessage));
            }

            return Ok(ApiResponse<List<GenreDto>>.CreateSuccess(result.Value!));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetGenres endpoint error");
            return StatusCode(500, ApiResponse.CreateFailure("Bir hata oluştu"));
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateGenre([FromBody] CreateGenreDto createDto, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _movieService.CreateGenreAsync(createDto, cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse.CreateFailure(result.ErrorMessage));
            }

            return Ok(ApiResponse<GenreDto>.CreateSuccess(result.Value!, "Tür başarıyla oluşturuldu"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateGenre endpoint error");
            return StatusCode(500, ApiResponse.CreateFailure("Bir hata oluştu"));
        }
    }
}
