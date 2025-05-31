using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CineSocial.Core.Application.Ports;
using CineSocial.Core.Application.DTOs.Groups;
using CineSocial.Adapters.WebAPI.DTOs.Responses;

namespace CineSocial.Adapters.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class GroupsController : ControllerBase
{
    private readonly IGroupService _groupService;
    private readonly ILogger<GroupsController> _logger;

    public GroupsController(IGroupService groupService, ILogger<GroupsController> logger)
    {
        _groupService = groupService;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetGroups(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] bool? isPrivate = null)
    {
        try
        {
            var result = await _groupService.GetGroupsAsync(page, pageSize, search, isPrivate);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse.CreateFailure(result.ErrorMessage));
            }

            return Ok(ApiResponse<PaginatedResponse<GroupSummaryDto>>.CreateSuccess(
                new PaginatedResponse<GroupSummaryDto>
                {
                    Items = result.Value!.Items,
                    TotalCount = result.Value.TotalCount,
                    Page = result.Value.Page,
                    PageSize = result.Value.PageSize
                }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetGroups endpoint error");
            return StatusCode(500, ApiResponse.CreateFailure("Bir hata oluştu"));
        }
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetGroup(Guid id)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var result = await _groupService.GetGroupByIdAsync(id, currentUserId);

            if (!result.IsSuccess)
            {
                return NotFound(ApiResponse.CreateFailure(result.ErrorMessage));
            }

            return Ok(ApiResponse<GroupDto>.CreateSuccess(result.Value!));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetGroup endpoint error");
            return StatusCode(500, ApiResponse.CreateFailure("Bir hata oluştu"));
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateGroup([FromBody] CreateGroupDto createDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return BadRequest(ApiResponse.CreateFailure("Kullanıcı kimliği bulunamadı"));
            }

            var result = await _groupService.CreateGroupAsync(userId.Value, createDto);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse.CreateFailure(result.ErrorMessage));
            }

            return CreatedAtAction(nameof(GetGroup), new { id = result.Value!.Id },
                ApiResponse<GroupDto>.CreateSuccess(result.Value, "Grup başarıyla oluşturuldu"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateGroup endpoint error");
            return StatusCode(500, ApiResponse.CreateFailure("Bir hata oluştu"));
        }
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateGroup(Guid id, [FromBody] UpdateGroupDto updateDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return BadRequest(ApiResponse.CreateFailure("Kullanıcı kimliği bulunamadı"));
            }

            var result = await _groupService.UpdateGroupAsync(userId.Value, id, updateDto);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse.CreateFailure(result.ErrorMessage));
            }

            return Ok(ApiResponse<GroupDto>.CreateSuccess(result.Value!, "Grup başarıyla güncellendi"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateGroup endpoint error");
            return StatusCode(500, ApiResponse.CreateFailure("Bir hata oluştu"));
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteGroup(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return BadRequest(ApiResponse.CreateFailure("Kullanıcı kimliği bulunamadı"));
            }

            var result = await _groupService.DeleteGroupAsync(userId.Value, id);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse.CreateFailure(result.ErrorMessage));
            }

            return Ok(ApiResponse.CreateSuccess("Grup başarıyla silindi"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DeleteGroup endpoint error");
            return StatusCode(500, ApiResponse.CreateFailure("Bir hata oluştu"));
        }
    }

    [HttpGet("{id}/members")]
    [AllowAnonymous]
    public async Task<IActionResult> GetGroupMembers(
        Guid id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var result = await _groupService.GetGroupMembersAsync(id, page, pageSize);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse.CreateFailure(result.ErrorMessage));
            }

            return Ok(ApiResponse<PaginatedResponse<GroupMemberDto>>.CreateSuccess(
                new PaginatedResponse<GroupMemberDto>
                {
                    Items = result.Value!.Items,
                    TotalCount = result.Value.TotalCount,
                    Page = result.Value.Page,
                    PageSize = result.Value.PageSize
                }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetGroupMembers endpoint error");
            return StatusCode(500, ApiResponse.CreateFailure("Bir hata oluştu"));
        }
    }

    [HttpPost("{id}/join")]
    [Authorize]
    public async Task<IActionResult> JoinGroup(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return BadRequest(ApiResponse.CreateFailure("Kullanıcı kimliği bulunamadı"));
            }

            var result = await _groupService.JoinGroupAsync(userId.Value, id);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse.CreateFailure(result.ErrorMessage));
            }

            return Ok(ApiResponse.CreateSuccess("Gruba başarıyla katıldınız"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "JoinGroup endpoint error");
            return StatusCode(500, ApiResponse.CreateFailure("Bir hata oluştu"));
        }
    }

    [HttpPost("{id}/leave")]
    [Authorize]
    public async Task<IActionResult> LeaveGroup(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return BadRequest(ApiResponse.CreateFailure("Kullanıcı kimliği bulunamadı"));
            }

            var result = await _groupService.LeaveGroupAsync(userId.Value, id);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse.CreateFailure(result.ErrorMessage));
            }

            return Ok(ApiResponse.CreateSuccess("Gruptan başarıyla ayrıldınız"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LeaveGroup endpoint error");
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
