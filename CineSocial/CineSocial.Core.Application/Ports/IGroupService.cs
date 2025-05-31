using CineSocial.Core.Application.DTOs.Common;
using CineSocial.Core.Application.DTOs.Groups;

namespace CineSocial.Core.Application.Ports;

public interface IGroupService
{
    Task<Result<PagedResult<GroupSummaryDto>>> GetGroupsAsync(int page = 1, int pageSize = 20, string? search = null, bool? isPrivate = null);
    Task<Result<GroupDto>> GetGroupByIdAsync(Guid id, Guid? currentUserId = null);
    Task<Result<GroupDto>> CreateGroupAsync(Guid userId, CreateGroupDto createDto);
    Task<Result<GroupDto>> UpdateGroupAsync(Guid userId, Guid groupId, UpdateGroupDto updateDto);
    Task<Result> DeleteGroupAsync(Guid userId, Guid groupId);
    Task<Result<PagedResult<GroupMemberDto>>> GetGroupMembersAsync(Guid groupId, int page = 1, int pageSize = 20);
    Task<Result> JoinGroupAsync(Guid userId, Guid groupId);
    Task<Result> LeaveGroupAsync(Guid userId, Guid groupId);
    Task<Result> UpdateMemberRoleAsync(Guid userId, Guid groupId, Guid memberId, GroupRole newRole);
    Task<Result> BanMemberAsync(Guid userId, Guid groupId, Guid memberId, string? reason = null, DateTime? expiresAt = null);
    Task<Result> UnbanMemberAsync(Guid userId, Guid groupId, Guid memberId);
    Task<Result<bool>> IsUserMemberAsync(Guid userId, Guid groupId);
    Task<Result<bool>> IsUserBannedAsync(Guid userId, Guid groupId);
}

