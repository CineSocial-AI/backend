using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CineSocial.Core.Application.Ports;
using CineSocial.Core.Application.DTOs.Posts;
using CineSocial.Adapters.WebAPI.DTOs.Responses;

namespace CineSocial.Adapters.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PostsController : ControllerBase
{
    private readonly IPostService _postService;
    private readonly ILogger<PostsController> _logger;

    public PostsController(IPostService postService, ILogger<PostsController> logger)
    {
        _postService = postService;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetPosts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? groupId = null,
        [FromQuery] Guid? userId = null,
        [FromQuery] string? search = null,
        [FromQuery] string? sortBy = null)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var result = await _postService.GetPostsAsync(page, pageSize, groupId, userId, search, sortBy);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse.CreateFailure(result.ErrorMessage));
            }

            return Ok(ApiResponse<PaginatedResponse<PostSummaryDto>>.CreateSuccess(
                new PaginatedResponse<PostSummaryDto>
                {
                    Items = result.Value!.Items,
                    TotalCount = result.Value.TotalCount,
                    Page = result.Value.Page,
                    PageSize = result.Value.PageSize
                }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetPosts endpoint error");
            return StatusCode(500, ApiResponse.CreateFailure("Bir hata oluştu"));
        }
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPost(Guid id)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var result = await _postService.GetPostByIdAsync(id, currentUserId);

            if (!result.IsSuccess)
            {
                return NotFound(ApiResponse.CreateFailure(result.ErrorMessage));
            }

            return Ok(ApiResponse<PostDto>.CreateSuccess(result.Value!));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetPost endpoint error");
            return StatusCode(500, ApiResponse.CreateFailure("Bir hata oluştu"));
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostDto createDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return BadRequest(ApiResponse.CreateFailure("Kullanıcı kimliği bulunamadı"));
            }

            var result = await _postService.CreatePostAsync(userId.Value, createDto);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse.CreateFailure(result.ErrorMessage));
            }

            return CreatedAtAction(nameof(GetPost), new { id = result.Value!.Id },
                ApiResponse<PostDto>.CreateSuccess(result.Value, "Post başarıyla oluşturuldu"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreatePost endpoint error");
            return StatusCode(500, ApiResponse.CreateFailure("Bir hata oluştu"));
        }
    }

    [HttpGet("{id}/comments")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPostComments(
        Guid id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var result = await _postService.GetPostCommentsAsync(id, page, pageSize, currentUserId);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse.CreateFailure(result.ErrorMessage));
            }

            return Ok(ApiResponse<PaginatedResponse<PostCommentDto>>.CreateSuccess(
                new PaginatedResponse<PostCommentDto>
                {
                    Items = result.Value!.Items,
                    TotalCount = result.Value.TotalCount,
                    Page = result.Value.Page,
                    PageSize = result.Value.PageSize
                }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetPostComments endpoint error");
            return StatusCode(500, ApiResponse.CreateFailure("Bir hata oluştu"));
        }
    }

    [HttpPost("comments")]
    [Authorize]
    public async Task<IActionResult> CreateComment([FromBody] CreatePostCommentDto createDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return BadRequest(ApiResponse.CreateFailure("Kullanıcı kimliği bulunamadı"));
            }

            var result = await _postService.CreateCommentAsync(userId.Value, createDto);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse.CreateFailure(result.ErrorMessage));
            }

            return Ok(ApiResponse<PostCommentDto>.CreateSuccess(result.Value!, "Yorum başarıyla oluşturuldu"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateComment endpoint error");
            return StatusCode(500, ApiResponse.CreateFailure("Bir hata oluştu"));
        }
    }

    [HttpPost("{id}/react")]
    [Authorize]
    public async Task<IActionResult> ReactToPost(Guid id, [FromBody] ReactionRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return BadRequest(ApiResponse.CreateFailure("Kullanıcı kimliği bulunamadı"));
            }

            var result = await _postService.ReactToPostAsync(userId.Value, id, request.Type);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse.CreateFailure(result.ErrorMessage));
            }

            return Ok(ApiResponse.CreateSuccess("Reaksiyon başarıyla eklendi"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ReactToPost endpoint error");
            return StatusCode(500, ApiResponse.CreateFailure("Bir hata oluştu"));
        }
    }

    [HttpGet("trending")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTrendingPosts([FromQuery] int count = 10)
    {
        try
        {
            var result = await _postService.GetTrendingPostsAsync(count);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse.CreateFailure(result.ErrorMessage));
            }

            return Ok(ApiResponse<List<PostSummaryDto>>.CreateSuccess(result.Value!));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetTrendingPosts endpoint error");
            return StatusCode(500, ApiResponse.CreateFailure("Bir hata oluştu"));
        }
    }

    [HttpGet("feed")]
    [Authorize]
    public async Task<IActionResult> GetUserFeed([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return BadRequest(ApiResponse.CreateFailure("Kullanıcı kimliği bulunamadı"));
            }

            var result = await _postService.GetUserFeedAsync(userId.Value, page, pageSize);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse.CreateFailure(result.ErrorMessage));
            }

            return Ok(ApiResponse<List<PostSummaryDto>>.CreateSuccess(result.Value!));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetUserFeed endpoint error");
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

public class ReactionRequest
{
    public ReactionType Type { get; set; }
}
