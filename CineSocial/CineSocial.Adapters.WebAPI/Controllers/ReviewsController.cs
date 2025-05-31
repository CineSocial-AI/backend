using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CineSocial.Core.Application.Ports;
using CineSocial.Core.Application.DTOs.Reviews;
using CineSocial.Adapters.WebAPI.DTOs.Responses;

namespace CineSocial.Adapters.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;
    private readonly ILogger<ReviewsController> _logger;

    public ReviewsController(IReviewService reviewService, ILogger<ReviewsController> logger)
    {
        _reviewService = reviewService;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetReviews(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? movieId = null,
        [FromQuery] Guid? userId = null)
    {
        try
        {
            var result = await _reviewService.GetReviewsAsync(page, pageSize, movieId, userId);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse.CreateFailure(result.ErrorMessage));
            }

            return Ok(ApiResponse<PaginatedResponse<ReviewDto>>.CreateSuccess(
                new PaginatedResponse<ReviewDto>
                {
                    Items = result.Value!.Items,
                    TotalCount = result.Value.TotalCount,
                    Page = result.Value.Page,
                    PageSize = result.Value.PageSize
                }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetReviews endpoint error");
            return StatusCode(500, ApiResponse.CreateFailure("Bir hata oluştu"));
        }
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetReview(Guid id)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var result = await _reviewService.GetReviewByIdAsync(id, currentUserId);

            if (!result.IsSuccess)
            {
                return NotFound(ApiResponse.CreateFailure(result.ErrorMessage));
            }

            return Ok(ApiResponse<ReviewDto>.CreateSuccess(result.Value!));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetReview endpoint error");
            return StatusCode(500, ApiResponse.CreateFailure("Bir hata oluştu"));
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto createDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return BadRequest(ApiResponse.CreateFailure("Kullanıcı kimliği bulunamadı"));
            }

            var result = await _reviewService.CreateReviewAsync(userId.Value, createDto);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse.CreateFailure(result.ErrorMessage));
            }

            return CreatedAtAction(nameof(GetReview), new { id = result.Value!.Id },
                ApiResponse<ReviewDto>.CreateSuccess(result.Value, "İnceleme başarıyla oluşturuldu"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateReview endpoint error");
            return StatusCode(500, ApiResponse.CreateFailure("Bir hata oluştu"));
        }
    }

    [HttpPost("{id}/like")]
    [Authorize]
    public async Task<IActionResult> LikeReview(Guid id, [FromBody] bool isLike = true)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return BadRequest(ApiResponse.CreateFailure("Kullanıcı kimliği bulunamadı"));
            }

            var result = await _reviewService.LikeReviewAsync(userId.Value, id, isLike);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse.CreateFailure(result.ErrorMessage));
            }

            return Ok(ApiResponse.CreateSuccess(isLike ? "İnceleme beğenildi" : "İnceleme beğenilmedi"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LikeReview endpoint error");
            return StatusCode(500, ApiResponse.CreateFailure("Bir hata oluştu"));
        }
    }

    [HttpGet("{id}/comments")]
    [AllowAnonymous]
    public async Task<IActionResult> GetComments(Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var result = await _reviewService.GetCommentsAsync(id, page, pageSize, currentUserId);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse.CreateFailure(result.ErrorMessage));
            }

            return Ok(ApiResponse<PaginatedResponse<CommentDto>>.CreateSuccess(
                new PaginatedResponse<CommentDto>
                {
                    Items = result.Value!.Items,
                    TotalCount = result.Value.TotalCount,
                    Page = result.Value.Page,
                    PageSize = result.Value.PageSize
                }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetComments endpoint error");
            return StatusCode(500, ApiResponse.CreateFailure("Bir hata oluştu"));
        }
    }

    [HttpPost("comments")]
    [Authorize]
    public async Task<IActionResult> CreateComment([FromBody] CreateCommentDto createDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return BadRequest(ApiResponse.CreateFailure("Kullanıcı kimliği bulunamadı"));
            }

            var result = await _reviewService.CreateCommentAsync(userId.Value, createDto);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse.CreateFailure(result.ErrorMessage));
            }

            return Ok(ApiResponse<CommentDto>.CreateSuccess(result.Value!, "Yorum başarıyla oluşturuldu"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateComment endpoint error");
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
