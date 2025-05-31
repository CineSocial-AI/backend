using Microsoft.EntityFrameworkCore;
using AutoMapper;
using CineSocial.Core.Application.Ports;
using CineSocial.Core.Application.DTOs.Common;
using CineSocial.Core.Application.DTOs.Groups;
using CineSocial.Core.Domain.Entities;
using CineSocial.Adapters.Infrastructure.Database;
using DomainGroupRole = CineSocial.Core.Domain.Entities.GroupRole;
using DtoGroupRole = CineSocial.Core.Application.DTOs.Groups.GroupRole;

namespace CineSocial.Adapters.Infrastructure.Services;

public class GroupService : IGroupService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GroupService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<GroupSummaryDto>>> GetGroupsAsync(int page = 1, int pageSize = 20, string? search = null, bool? isPrivate = null)
    {
        try
        {
            var query = _context.Groups.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(g => g.Name.Contains(search) || (g.Description != null && g.Description.Contains(search)));
            }

            if (isPrivate.HasValue)
            {
                query = query.Where(g => g.IsPrivate == isPrivate.Value);
            }

            query = query.OrderByDescending(g => g.MemberCount).ThenBy(g => g.Name);

            var totalCount = await query.CountAsync();
            var groups = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var groupDtos = _mapper.Map<List<GroupSummaryDto>>(groups);
            var result = new PagedResult<GroupSummaryDto>(groupDtos, totalCount, page, pageSize);
            return Result<PagedResult<GroupSummaryDto>>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<PagedResult<GroupSummaryDto>>.Failure($"Error getting groups: {ex.Message}");
        }
    }

    public async Task<Result<GroupDto>> GetGroupByIdAsync(Guid id, Guid? currentUserId = null)
    {
        try
        {
            var group = await _context.Groups
                .Include(g => g.CreatedBy)
                .Include(g => g.Members)
                .ThenInclude(m => m.User)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (group == null)
            {
                return Result<GroupDto>.Failure("Group not found");
            }

            var groupDto = _mapper.Map<GroupDto>(group);

            if (currentUserId.HasValue)
            {
                var membership = group.Members.FirstOrDefault(m => m.UserId == currentUserId.Value && m.IsActive);
                groupDto.IsCurrentUserMember = membership != null;
                groupDto.CurrentUserRole = membership != null ? (DtoGroupRole?)membership.Role : null;

                var ban = await _context.GroupBans
                    .FirstOrDefaultAsync(b => b.GroupId == id && b.UserId == currentUserId.Value && b.IsActive);
                groupDto.IsCurrentUserBanned = ban != null;
            }

            return Result<GroupDto>.Success(groupDto);
        }
        catch (Exception ex)
        {
            return Result<GroupDto>.Failure($"Error getting group: {ex.Message}");
        }
    }

    public async Task<Result<GroupDto>> CreateGroupAsync(Guid userId, CreateGroupDto createDto)
    {
        try
        {
            var group = _mapper.Map<Group>(createDto);
            group.Id = Guid.NewGuid();
            group.CreatedById = userId;
            group.CreatedAt = DateTime.UtcNow;

            _context.Groups.Add(group);

            var membership = new GroupMember
            {
                Id = Guid.NewGuid(),
                GroupId = group.Id,
                UserId = userId,
                Role = DomainGroupRole.Owner,
                JoinedAt = DateTime.UtcNow,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.GroupMembers.Add(membership);
            await _context.SaveChangesAsync();

            return await GetGroupByIdAsync(group.Id, userId);
        }
        catch (Exception ex)
        {
            return Result<GroupDto>.Failure($"Error creating group: {ex.Message}");
        }
    }

    public async Task<Result<GroupDto>> UpdateGroupAsync(Guid userId, Guid groupId, UpdateGroupDto updateDto)
    {
        try
        {
            var group = await _context.Groups
                .Include(g => g.Members)
                .FirstOrDefaultAsync(g => g.Id == groupId);

            if (group == null)
            {
                return Result<GroupDto>.Failure("Group not found");
            }

            var membership = group.Members.FirstOrDefault(m => m.UserId == userId && m.IsActive);
            if (membership == null || (membership.Role != DomainGroupRole.Owner && membership.Role != DomainGroupRole.Admin))
            {
                return Result<GroupDto>.Failure("Insufficient permissions");
            }

            _mapper.Map(updateDto, group);
            group.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return await GetGroupByIdAsync(groupId, userId);
        }
        catch (Exception ex)
        {
            return Result<GroupDto>.Failure($"Error updating group: {ex.Message}");
        }
    }

    public async Task<Result> DeleteGroupAsync(Guid userId, Guid groupId)
    {
        try
        {
            var group = await _context.Groups
                .Include(g => g.Members)
                .FirstOrDefaultAsync(g => g.Id == groupId);

            if (group == null)
            {
                return Result.Failure("Group not found");
            }

            var membership = group.Members.FirstOrDefault(m => m.UserId == userId && m.IsActive);
            if (membership == null || membership.Role != DomainGroupRole.Owner)
            {
                return Result.Failure("Only group owner can delete the group");
            }

            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error deleting group: {ex.Message}");
        }
    }

    public async Task<Result<PagedResult<GroupMemberDto>>> GetGroupMembersAsync(Guid groupId, int page = 1, int pageSize = 20)
    {
        try
        {
            var query = _context.GroupMembers
                .Include(m => m.User)
                .Where(m => m.GroupId == groupId && m.IsActive)
                .OrderByDescending(m => m.Role)
                .ThenBy(m => m.JoinedAt);

            var totalCount = await query.CountAsync();
            var members = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var memberDtos = _mapper.Map<List<GroupMemberDto>>(members);
            var result = new PagedResult<GroupMemberDto>(memberDtos, totalCount, page, pageSize);
            return Result<PagedResult<GroupMemberDto>>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<PagedResult<GroupMemberDto>>.Failure($"Error getting group members: {ex.Message}");
        }
    }

    public async Task<Result> JoinGroupAsync(Guid userId, Guid groupId)
    {
        try
        {
            var group = await _context.Groups.FindAsync(groupId);
            if (group == null)
            {
                return Result.Failure("Group not found");
            }

            var existingMembership = await _context.GroupMembers
                .FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == userId);

            if (existingMembership != null)
            {
                if (existingMembership.IsActive)
                {
                    return Result.Failure("User is already a member");
                }

                existingMembership.IsActive = true;
                existingMembership.JoinedAt = DateTime.UtcNow;
            }
            else
            {
                var membership = new GroupMember
                {
                    Id = Guid.NewGuid(),
                    GroupId = groupId,
                    UserId = userId,
                    Role = DomainGroupRole.Member,
                    JoinedAt = DateTime.UtcNow,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.GroupMembers.Add(membership);
            }

            group.MemberCount++;
            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error joining group: {ex.Message}");
        }
    }

    public async Task<Result> LeaveGroupAsync(Guid userId, Guid groupId)
    {
        try
        {
            var membership = await _context.GroupMembers
                .FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == userId && m.IsActive);

            if (membership == null)
            {
                return Result.Failure("User is not a member of this group");
            }

            if (membership.Role == DomainGroupRole.Owner)
            {
                return Result.Failure("Group owner cannot leave the group");
            }

            membership.IsActive = false;

            var group = await _context.Groups.FindAsync(groupId);
            if (group != null)
            {
                group.MemberCount--;
            }

            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error leaving group: {ex.Message}");
        }
    }

    public async Task<Result> UpdateMemberRoleAsync(Guid userId, Guid groupId, Guid memberId, DtoGroupRole newRole)
    {
        try
        {
            var userMembership = await _context.GroupMembers
                .FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == userId && m.IsActive);

            if (userMembership == null || (userMembership.Role != DomainGroupRole.Owner && userMembership.Role != DomainGroupRole.Admin))
            {
                return Result.Failure("Insufficient permissions");
            }

            var targetMembership = await _context.GroupMembers
                .FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == memberId && m.IsActive);

            if (targetMembership == null)
            {
                return Result.Failure("Member not found");
            }

            if (newRole == DtoGroupRole.Owner)
            {
                return Result.Failure("Cannot assign owner role");
            }

            targetMembership.Role = (DomainGroupRole)newRole;
            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error updating member role: {ex.Message}");
        }
    }

    public async Task<Result> BanMemberAsync(Guid userId, Guid groupId, Guid memberId, string? reason = null, DateTime? expiresAt = null)
    {
        try
        {
            var userMembership = await _context.GroupMembers
                .FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == userId && m.IsActive);

            if (userMembership == null || (userMembership.Role != DomainGroupRole.Owner && userMembership.Role != DomainGroupRole.Admin && userMembership.Role != DomainGroupRole.Moderator))
            {
                return Result.Failure("Insufficient permissions");
            }

            var targetMembership = await _context.GroupMembers
                .FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == memberId && m.IsActive);

            if (targetMembership != null)
            {
                targetMembership.IsActive = false;
            }

            var ban = new GroupBan
            {
                Id = Guid.NewGuid(),
                GroupId = groupId,
                UserId = memberId,
                BannedById = userId,
                Reason = reason,
                ExpiresAt = expiresAt,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.GroupBans.Add(ban);

            var group = await _context.Groups.FindAsync(groupId);
            if (group != null && targetMembership != null)
            {
                group.MemberCount--;
            }

            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error banning member: {ex.Message}");
        }
    }

    public async Task<Result> UnbanMemberAsync(Guid userId, Guid groupId, Guid memberId)
    {
        try
        {
            var userMembership = await _context.GroupMembers
                .FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == userId && m.IsActive);

            if (userMembership == null || (userMembership.Role != DomainGroupRole.Owner && userMembership.Role != DomainGroupRole.Admin && userMembership.Role != DomainGroupRole.Moderator))
            {
                return Result.Failure("Insufficient permissions");
            }

            var ban = await _context.GroupBans
                .FirstOrDefaultAsync(b => b.GroupId == groupId && b.UserId == memberId && b.IsActive);

            if (ban == null)
            {
                return Result.Failure("User is not banned");
            }

            ban.IsActive = false;
            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error unbanning member: {ex.Message}");
        }
    }

    public async Task<Result<bool>> IsUserMemberAsync(Guid userId, Guid groupId)
    {
        try
        {
            var isMember = await _context.GroupMembers
                .AnyAsync(m => m.GroupId == groupId && m.UserId == userId && m.IsActive);

            return Result<bool>.Success(isMember);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Error checking membership: {ex.Message}");
        }
    }

    public async Task<Result<bool>> IsUserBannedAsync(Guid userId, Guid groupId)
    {
        try
        {
            var isBanned = await _context.GroupBans
                .AnyAsync(b => b.GroupId == groupId && b.UserId == userId && b.IsActive);

            return Result<bool>.Success(isBanned);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Error checking ban status: {ex.Message}");
        }
    }
}
