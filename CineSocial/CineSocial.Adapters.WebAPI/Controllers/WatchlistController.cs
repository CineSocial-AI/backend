using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CineSocial.Core.Application.Ports;
using CineSocial.Core.Application.DTOs.Watchlists;
using CineSocial.Adapters.WebAPI.DTOs.Responses;

namespace CineSocial.Adapters.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class WatchlistController : ControllerBase
{
    private readonly IWatchlistService _watchlistService;
    private readonly ILogger<WatchlistController> _logger;

    public WatchlistController(IWatchlistService watchlistService, ILogger<WatchlistController> logger)
    {
        _watchlistService = watchlistService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetWatchlist(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool? isWatched = null)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return BadRequest(ApiResponse.CreateFailure("Kullanıcı kimliği bulunamadı"));
            }

            var result = await _watchlistService.GetUserWatchlistAsync(userId.Value, page, pageSize, isWatched);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse.CreateFailure(result.ErrorMessage));
            }

            return Ok(ApiResponse<PaginatedResponse<WatchlistDto>>.CreateSuccess(
                new PaginatedResponse<WatchlistDto>
                {
                    Items = result.Value!.Items,
                    TotalCount = result.Value.TotalCount,
                    Page = result.Value.Page,
                    PageSize = result.Value.PageSize
                }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetWatchlist endpoint error");
            return StatusCode(500, ApiResponse.CreateFailure("Bir hata oluştu"));
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddToWatchlist([FromBody] AddToWatchlistDto addDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return BadRequest(ApiResponse.CreateFailure("Kullanıcı kimliği bulunamadı"));
            }

            var result = await _watchlistService.AddToWatchlistAsync(userId.Value, addDto);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse.CreateFailure(result.ErrorMessage));
            }

            return Ok(ApiResponse<WatchlistDto>.CreateSuccess(result.Value!, "Film izleme listesine eklendi"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AddToWatchlist endpoint error");
            return StatusCode(500, ApiResponse.CreateFailure("Bir hata oluştu"));
        }
    }

    [HttpGet("check/{movieId}")]
    public async Task<IActionResult> CheckWatchlist(Guid movieId)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return BadRequest(ApiResponse.CreateFailure("Kullanıcı kimliği bulunamadı"));
            }

            var result = await _watchlistService.IsInWatchlistAsync(userId.Value, movieId);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse.CreateFailure(result.ErrorMessage));
            }

            return Ok(ApiResponse<bool>.CreateSuccess(result.Value));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CheckWatchlist endpoint error");
            return StatusCode(500, ApiResponse.CreateFailure("Bir hata oluştu"));
        }
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }
        return null;
    }
}
