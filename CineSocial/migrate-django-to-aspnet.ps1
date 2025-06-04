# Reddit-like Platform Entity Creation Script for CineSocial
# Run this script from: D:\code\CineSocial\backend\CineSocial\

Write-Host "Creating Reddit-like Platform Entities..." -ForegroundColor Green

# ================================
# 1. DOMAIN ENTITIES
# ================================

Write-Host "Creating Domain Entities..." -ForegroundColor Yellow

# Group Entity
Write-Host "Creating Group Entity..." -ForegroundColor Cyan
$groupEntityContent = @'
using CineSocial.Core.Domain.Common;

namespace CineSocial.Core.Domain.Entities;

public class Group : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Rules { get; set; }
    public string? IconUrl { get; set; }
    public string? BannerUrl { get; set; }
    public bool IsPrivate { get; set; }
    public bool RequireApproval { get; set; }
    public bool IsNsfw { get; set; }
    public int MemberCount { get; set; }
    public Guid CreatedById { get; set; }

    public virtual User CreatedBy { get; set; } = null!;
    public virtual ICollection<GroupMember> Members { get; set; } = new List<GroupMember>();
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
    public virtual ICollection<GroupBan> Bans { get; set; } = new List<GroupBan>();
}
'@
Set-Content -Path "CineSocial.Core.Domain\Entities\Group.cs" -Value $groupEntityContent

# GroupMember Entity
Write-Host "Creating GroupMember Entity..." -ForegroundColor Cyan
$groupMemberContent = @'
using CineSocial.Core.Domain.Common;

namespace CineSocial.Core.Domain.Entities;

public class GroupMember : BaseEntity
{
    public Guid GroupId { get; set; }
    public Guid UserId { get; set; }
    public GroupRole Role { get; set; }
    public DateTime JoinedAt { get; set; }
    public bool IsActive { get; set; }

    public virtual Group Group { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}

public enum GroupRole
{
    Member = 1,
    Moderator = 2,
    Admin = 3,
    Owner = 4
}
'@
Set-Content -Path "CineSocial.Core.Domain\Entities\GroupMember.cs" -Value $groupMemberContent

# Post Entity
Write-Host "Creating Post Entity..." -ForegroundColor Cyan
$postEntityContent = @'
using CineSocial.Core.Domain.Common;

namespace CineSocial.Core.Domain.Entities;

public class Post : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
    public PostType Type { get; set; }
    public string? Url { get; set; }
    public bool IsNsfw { get; set; }
    public bool IsSpoiler { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsLocked { get; set; }
    public int UpvoteCount { get; set; }
    public int DownvoteCount { get; set; }
    public int CommentCount { get; set; }
    public int ViewCount { get; set; }
    public Guid AuthorId { get; set; }
    public Guid GroupId { get; set; }

    public virtual User Author { get; set; } = null!;
    public virtual Group Group { get; set; } = null!;
    public virtual ICollection<PostMedia> Media { get; set; } = new List<PostMedia>();
    public virtual ICollection<PostReaction> Reactions { get; set; } = new List<PostReaction>();
    public virtual ICollection<PostComment> Comments { get; set; } = new List<PostComment>();
    public virtual ICollection<PostTag> Tags { get; set; } = new List<PostTag>();
    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    public int GetScore() => UpvoteCount - DownvoteCount;
}

public enum PostType
{
    Text = 1,
    Image = 2,
    Video = 3,
    Link = 4,
    Poll = 5
}
'@
Set-Content -Path "CineSocial.Core.Domain\Entities\Post.cs" -Value $postEntityContent

# PostMedia Entity
Write-Host "Creating PostMedia Entity..." -ForegroundColor Cyan
$postMediaContent = @'
using CineSocial.Core.Domain.Common;

namespace CineSocial.Core.Domain.Entities;

public class PostMedia : BaseEntity
{
    public Guid PostId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public MediaType Type { get; set; }
    public long FileSize { get; set; }
    public string? MimeType { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public int? Duration { get; set; }
    public int Order { get; set; }

    public virtual Post Post { get; set; } = null!;
}

public enum MediaType
{
    Image = 1,
    Video = 2,
    Audio = 3,
    Document = 4
}
'@
Set-Content -Path "CineSocial.Core.Domain\Entities\PostMedia.cs" -Value $postMediaContent

# PostComment Entity
Write-Host "Creating PostComment Entity..." -ForegroundColor Cyan
$postCommentContent = @'
using CineSocial.Core.Domain.Common;

namespace CineSocial.Core.Domain.Entities;

public class PostComment : BaseEntity
{
    public Guid PostId { get; set; }
    public Guid AuthorId { get; set; }
    public Guid? ParentCommentId { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public bool IsEdited { get; set; }
    public int UpvoteCount { get; set; }
    public int DownvoteCount { get; set; }
    public int ReplyCount { get; set; }

    public virtual Post Post { get; set; } = null!;
    public virtual User Author { get; set; } = null!;
    public virtual PostComment? ParentComment { get; set; }
    public virtual ICollection<PostComment> Replies { get; set; } = new List<PostComment>();
    public virtual ICollection<CommentReaction> Reactions { get; set; } = new List<CommentReaction>();
    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    public int GetScore() => UpvoteCount - DownvoteCount;
}
'@
Set-Content -Path "CineSocial.Core.Domain\Entities\PostComment.cs" -Value $postCommentContent

# PostReaction Entity
Write-Host "Creating PostReaction Entity..." -ForegroundColor Cyan
$postReactionContent = @'
using CineSocial.Core.Domain.Common;

namespace CineSocial.Core.Domain.Entities;

public class PostReaction : BaseEntity
{
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public ReactionType Type { get; set; }

    public virtual Post Post { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}

public enum ReactionType
{
    Upvote = 1,
    Downvote = -1
}
'@
Set-Content -Path "CineSocial.Core.Domain\Entities\PostReaction.cs" -Value $postReactionContent

# CommentReaction Entity (Update existing CommentLike)
Write-Host "Creating CommentReaction Entity..." -ForegroundColor Cyan
$commentReactionContent = @'
using CineSocial.Core.Domain.Common;

namespace CineSocial.Core.Domain.Entities;

public class CommentReaction : BaseEntity
{
    public Guid CommentId { get; set; }
    public Guid UserId { get; set; }
    public ReactionType Type { get; set; }

    public virtual PostComment Comment { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}
'@
Set-Content -Path "CineSocial.Core.Domain\Entities\CommentReaction.cs" -Value $commentReactionContent

# GroupBan Entity
Write-Host "Creating GroupBan Entity..." -ForegroundColor Cyan
$groupBanContent = @'
using CineSocial.Core.Domain.Common;

namespace CineSocial.Core.Domain.Entities;

public class GroupBan : BaseEntity
{
    public Guid GroupId { get; set; }
    public Guid UserId { get; set; }
    public Guid BannedById { get; set; }
    public string? Reason { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; }

    public virtual Group Group { get; set; } = null!;
    public virtual User User { get; set; } = null!;
    public virtual User BannedBy { get; set; } = null!;
}
'@
Set-Content -Path "CineSocial.Core.Domain\Entities\GroupBan.cs" -Value $groupBanContent

# UserBlock Entity
Write-Host "Creating UserBlock Entity..." -ForegroundColor Cyan
$userBlockContent = @'
using CineSocial.Core.Domain.Common;

namespace CineSocial.Core.Domain.Entities;

public class UserBlock : BaseEntity
{
    public Guid BlockerId { get; set; }
    public Guid BlockedId { get; set; }

    public virtual User Blocker { get; set; } = null!;
    public virtual User Blocked { get; set; } = null!;
}
'@
Set-Content -Path "CineSocial.Core.Domain\Entities\UserBlock.cs" -Value $userBlockContent

# Report Entity
Write-Host "Creating Report Entity..." -ForegroundColor Cyan
$reportContent = @'
using CineSocial.Core.Domain.Common;

namespace CineSocial.Core.Domain.Entities;

public class Report : BaseEntity
{
    public Guid ReporterId { get; set; }
    public ReportTargetType TargetType { get; set; }
    public Guid TargetId { get; set; }
    public ReportReason Reason { get; set; }
    public string? Details { get; set; }
    public ReportStatus Status { get; set; }
    public Guid? ReviewedById { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewNotes { get; set; }

    public virtual User Reporter { get; set; } = null!;
    public virtual User? ReviewedBy { get; set; }
}

public enum ReportTargetType
{
    Post = 1,
    Comment = 2,
    User = 3,
    Group = 4
}

public enum ReportReason
{
    Spam = 1,
    Harassment = 2,
    HateSpeech = 3,
    Violence = 4,
    Inappropriate = 5,
    Copyright = 6,
    Other = 7
}

public enum ReportStatus
{
    Pending = 1,
    Reviewed = 2,
    Resolved = 3,
    Dismissed = 4
}
'@
Set-Content -Path "CineSocial.Core.Domain\Entities\Report.cs" -Value $reportContent

# PostTag Entity
Write-Host "Creating PostTag Entity..." -ForegroundColor Cyan
$postTagContent = @'
using CineSocial.Core.Domain.Common;

namespace CineSocial.Core.Domain.Entities;

public class PostTag : BaseEntity
{
    public Guid PostId { get; set; }
    public string Tag { get; set; } = string.Empty;

    public virtual Post Post { get; set; } = null!;
}
'@
Set-Content -Path "CineSocial.Core.Domain\Entities\PostTag.cs" -Value $postTagContent

# Following Entity
Write-Host "Creating Following Entity..." -ForegroundColor Cyan
$followingContent = @'
using CineSocial.Core.Domain.Common;

namespace CineSocial.Core.Domain.Entities;

public class Following : BaseEntity
{
    public Guid FollowerId { get; set; }
    public Guid FollowingId { get; set; }

    public virtual User Follower { get; set; } = null!;
    public virtual User Following { get; set; } = null!;
}
'@
Set-Content -Path "CineSocial.Core.Domain\Entities\Following.cs" -Value $followingContent

# ================================
# 2. UPDATE USER ENTITY
# ================================

Write-Host "Updating User Entity..." -ForegroundColor Yellow
$userEntityContent = @'
using Microsoft.AspNetCore.Identity;
using CineSocial.Core.Domain.Common;

namespace CineSocial.Core.Domain.Entities;

public class User : IdentityUser<Guid>, IAuditableEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Bio { get; set; }
    public bool IsActive { get; set; } = true;
    public int KarmaPoints { get; set; } = 0;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    // Movie-related collections (existing)
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    public virtual ICollection<Watchlist> Watchlists { get; set; } = new List<Watchlist>();
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public virtual ICollection<ReviewLike> ReviewLikes { get; set; } = new List<ReviewLike>();
    public virtual ICollection<CommentLike> CommentLikes { get; set; } = new List<CommentLike>();

    // Reddit-like platform collections (new)
    public virtual ICollection<Group> CreatedGroups { get; set; } = new List<Group>();
    public virtual ICollection<GroupMember> GroupMemberships { get; set; } = new List<GroupMember>();
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
    public virtual ICollection<PostComment> PostComments { get; set; } = new List<PostComment>();
    public virtual ICollection<PostReaction> PostReactions { get; set; } = new List<PostReaction>();
    public virtual ICollection<CommentReaction> CommentReactions { get; set; } = new List<CommentReaction>();
    public virtual ICollection<GroupBan> GroupBans { get; set; } = new List<GroupBan>();
    public virtual ICollection<GroupBan> IssuedBans { get; set; } = new List<GroupBan>();
    public virtual ICollection<UserBlock> BlockedUsers { get; set; } = new List<UserBlock>();
    public virtual ICollection<UserBlock> BlockedByUsers { get; set; } = new List<UserBlock>();
    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();
    public virtual ICollection<Report> ReviewedReports { get; set; } = new List<Report>();
    public virtual ICollection<Following> Followers { get; set; } = new List<Following>();
    public virtual ICollection<Following> Followings { get; set; } = new List<Following>();

    public string GetFullName() => $"{FirstName} {LastName}".Trim();
}
'@
Set-Content -Path "CineSocial.Core.Domain\Entities\User.cs" -Value $userEntityContent

# ================================
# 3. CREATE DTOS
# ================================

Write-Host "Creating DTOs..." -ForegroundColor Yellow

# Create Groups DTOs folder
New-Item -Path "CineSocial.Core.Application\DTOs\Groups" -ItemType Directory -Force

# Group DTOs
Write-Host "Creating Group DTOs..." -ForegroundColor Cyan
$groupDtosContent = @'
using System.ComponentModel.DataAnnotations;

namespace CineSocial.Core.Application.DTOs.Groups;

public class GroupDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Rules { get; set; }
    public string? IconUrl { get; set; }
    public string? BannerUrl { get; set; }
    public bool IsPrivate { get; set; }
    public bool RequireApproval { get; set; }
    public bool IsNsfw { get; set; }
    public int MemberCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public GroupRole? CurrentUserRole { get; set; }
    public bool IsCurrentUserMember { get; set; }
    public bool IsCurrentUserBanned { get; set; }
}

public class GroupSummaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconUrl { get; set; }
    public int MemberCount { get; set; }
    public bool IsPrivate { get; set; }
    public bool IsNsfw { get; set; }
}

public class CreateGroupDto
{
    [Required]
    [MinLength(3)]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(2000)]
    public string? Rules { get; set; }

    public bool IsPrivate { get; set; }
    public bool RequireApproval { get; set; }
    public bool IsNsfw { get; set; }
}

public class UpdateGroupDto
{
    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(2000)]
    public string? Rules { get; set; }

    public bool IsPrivate { get; set; }
    public bool RequireApproval { get; set; }
    public bool IsNsfw { get; set; }
}

public class GroupMemberDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserFullName { get; set; } = string.Empty;
    public string? UserProfileImage { get; set; }
    public GroupRole Role { get; set; }
    public DateTime JoinedAt { get; set; }
    public bool IsActive { get; set; }
}

public enum GroupRole
{
    Member = 1,
    Moderator = 2,
    Admin = 3,
    Owner = 4
}
'@
Set-Content -Path "CineSocial.Core.Application\DTOs\Groups\GroupDtos.cs" -Value $groupDtosContent

# Create Posts DTOs folder
New-Item -Path "CineSocial.Core.Application\DTOs\Posts" -ItemType Directory -Force

# Post DTOs
Write-Host "Creating Post DTOs..." -ForegroundColor Cyan
$postDtosContent = @'
using System.ComponentModel.DataAnnotations;

namespace CineSocial.Core.Application.DTOs.Posts;

public class PostDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
    public PostType Type { get; set; }
    public string? Url { get; set; }
    public bool IsNsfw { get; set; }
    public bool IsSpoiler { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsLocked { get; set; }
    public int UpvoteCount { get; set; }
    public int DownvoteCount { get; set; }
    public int Score { get; set; }
    public int CommentCount { get; set; }
    public int ViewCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string? AuthorProfileImage { get; set; }
    
    public Guid GroupId { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public string? GroupIcon { get; set; }
    
    public List<PostMediaDto> Media { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    public ReactionType? CurrentUserReaction { get; set; }
}

public class PostSummaryDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
    public PostType Type { get; set; }
    public bool IsNsfw { get; set; }
    public bool IsSpoiler { get; set; }
    public int Score { get; set; }
    public int CommentCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public ReactionType? CurrentUserReaction { get; set; }
}

public class CreatePostDto
{
    [Required]
    [MinLength(5)]
    [MaxLength(300)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(10000)]
    public string? Content { get; set; }

    [Required]
    public PostType Type { get; set; }

    public string? Url { get; set; }
    public bool IsNsfw { get; set; }
    public bool IsSpoiler { get; set; }

    [Required]
    public Guid GroupId { get; set; }

    public List<string> Tags { get; set; } = new();
}

public class UpdatePostDto
{
    [MaxLength(10000)]
    public string? Content { get; set; }

    public bool IsNsfw { get; set; }
    public bool IsSpoiler { get; set; }
    public List<string> Tags { get; set; } = new();
}

public class PostMediaDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public MediaType Type { get; set; }
    public long FileSize { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public int? Duration { get; set; }
    public int Order { get; set; }
}

public class PostCommentDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public bool IsEdited { get; set; }
    public int UpvoteCount { get; set; }
    public int DownvoteCount { get; set; }
    public int Score { get; set; }
    public int ReplyCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string? AuthorProfileImage { get; set; }
    
    public Guid? ParentCommentId { get; set; }
    public List<PostCommentDto> Replies { get; set; } = new();
    public ReactionType? CurrentUserReaction { get; set; }
}

public class CreatePostCommentDto
{
    [Required]
    public Guid PostId { get; set; }

    public Guid? ParentCommentId { get; set; }

    [Required]
    [MinLength(1)]
    [MaxLength(2000)]
    public string Content { get; set; } = string.Empty;
}

public class UpdatePostCommentDto
{
    [Required]
    [MinLength(1)]
    [MaxLength(2000)]
    public string Content { get; set; } = string.Empty;
}

public enum PostType
{
    Text = 1,
    Image = 2,
    Video = 3,
    Link = 4,
    Poll = 5
}

public enum MediaType
{
    Image = 1,
    Video = 2,
    Audio = 3,
    Document = 4
}

public enum ReactionType
{
    Upvote = 1,
    Downvote = -1
}
'@
Set-Content -Path "CineSocial.Core.Application\DTOs\Posts\PostDtos.cs" -Value $postDtosContent

# ================================
# 4. CREATE PORTS (SERVICE INTERFACES)
# ================================

Write-Host "Creating Service Interfaces..." -ForegroundColor Yellow

# Group Service Interface
Write-Host "Creating IGroupService..." -ForegroundColor Cyan
$groupServiceContent = @'
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
'@
Set-Content -Path "CineSocial.Core.Application\Ports\IGroupService.cs" -Value $groupServiceContent

# Post Service Interface
Write-Host "Creating IPostService..." -ForegroundColor Cyan
$postServiceContent = @'
using CineSocial.Core.Application.DTOs.Common;
using CineSocial.Core.Application.DTOs.Posts;

namespace CineSocial.Core.Application.Ports;

public interface IPostService
{
    Task<Result<PagedResult<PostSummaryDto>>> GetPostsAsync(int page = 1, int pageSize = 20, Guid? groupId = null, Guid? userId = null, string? search = null, string? sortBy = null);
    Task<Result<PostDto>> GetPostByIdAsync(Guid id, Guid? currentUserId = null);
    Task<Result<PostDto>> CreatePostAsync(Guid userId, CreatePostDto createDto);
    Task<Result<PostDto>> UpdatePostAsync(Guid userId, Guid postId, UpdatePostDto updateDto);
    Task<Result> DeletePostAsync(Guid userId, Guid postId);
    Task<Result> ReactToPostAsync(Guid userId, Guid postId, ReactionType reactionType);
    Task<Result> RemovePostReactionAsync(Guid userId, Guid postId);
    Task<Result<PagedResult<PostCommentDto>>> GetPostCommentsAsync(Guid postId, int page = 1, int pageSize = 20, Guid? currentUserId = null);
    Task<Result<PostCommentDto>> CreateCommentAsync(Guid userId, CreatePostCommentDto createDto);
    Task<Result<PostCommentDto>> UpdateCommentAsync(Guid userId, Guid commentId, UpdatePostCommentDto updateDto);
    Task<Result> DeleteCommentAsync(Guid userId, Guid commentId);
    Task<Result> ReactToCommentAsync(Guid userId, Guid commentId, ReactionType reactionType);
    Task<Result> RemoveCommentReactionAsync(Guid userId, Guid commentId);
    Task<Result<List<PostSummaryDto>>> GetTrendingPostsAsync(int count = 10);
    Task<Result<List<PostSummaryDto>>> GetUserFeedAsync(Guid userId, int page = 1, int pageSize = 20);
}
'@
Set-Content -Path "CineSocial.Core.Application\Ports\IPostService.cs" -Value $postServiceContent

# ================================
# 5. CREATE MAPPING PROFILES
# ================================

Write-Host "Creating Mapping Profiles..." -ForegroundColor Yellow

# Group Mapping Profile
Write-Host "Creating GroupMappingProfile..." -ForegroundColor Cyan
$groupMappingContent = @'
using AutoMapper;
using CineSocial.Core.Domain.Entities;
using CineSocial.Core.Application.DTOs.Groups;

namespace CineSocial.Core.Application.Mapping;

public class GroupMappingProfile : Profile
{
    public GroupMappingProfile()
    {
        CreateMap<Group, GroupDto>()
            .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.CreatedBy.GetFullName()))
            .ForMember(dest => dest.CurrentUserRole, opt => opt.Ignore())
            .ForMember(dest => dest.IsCurrentUserMember, opt => opt.Ignore())
            .ForMember(dest => dest.IsCurrentUserBanned, opt => opt.Ignore());

        CreateMap<Group, GroupSummaryDto>();

        CreateMap<CreateGroupDto, Group>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
            .ForMember(dest => dest.MemberCount, opt => opt.MapFrom(src => 1))
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Members, opt => opt.Ignore())
            .ForMember(dest => dest.Posts, opt => opt.Ignore())
            .ForMember(dest => dest.Bans, opt => opt.Ignore());

        CreateMap<UpdateGroupDto, Group>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Name, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
            .ForMember(dest => dest.MemberCount, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Members, opt => opt.Ignore())
            .ForMember(dest => dest.Posts, opt => opt.Ignore())
            .ForMember(dest => dest.Bans, opt => opt.Ignore());

        CreateMap<GroupMember, GroupMemberDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
            .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User.GetFullName()))
            .ForMember(dest => dest.UserProfileImage, opt => opt.MapFrom(src => src.User.ProfileImageUrl));
    }
}
'@
Set-Content -Path "CineSocial.Core.Application\Mapping\GroupMappingProfile.cs" -Value $groupMappingContent

# Post Mapping Profile
Write-Host "Creating PostMappingProfile..." -ForegroundColor Cyan
$postMappingContent = @'
using AutoMapper;
using CineSocial.Core.Domain.Entities;
using CineSocial.Core.Application.DTOs.Posts;

namespace CineSocial.Core.Application.Mapping;

public class PostMappingProfile : Profile
{
    public PostMappingProfile()
    {
        CreateMap<Post, PostDto>()
            .ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.GetScore()))
            .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.UserName))
            .ForMember(dest => dest.AuthorProfileImage, opt => opt.MapFrom(src => src.Author.ProfileImageUrl))
            .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group.Name))
            .ForMember(dest => dest.GroupIcon, opt => opt.MapFrom(src => src.Group.IconUrl))
            .ForMember(dest => dest.Media, opt => opt.MapFrom(src => src.Media))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(t => t.Tag)))
            .ForMember(dest => dest.CurrentUserReaction, opt => opt.Ignore());

        CreateMap<Post, PostSummaryDto>()
            .ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.GetScore()))
            .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.UserName))
            .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group.Name))
            .ForMember(dest => dest.ThumbnailUrl, opt => opt.MapFrom(src => src.Media.FirstOrDefault() != null ? src.Media.FirstOrDefault()!.ThumbnailUrl : null))
            .ForMember(dest => dest.CurrentUserReaction, opt => opt.Ignore());

        CreateMap<CreatePostDto, Post>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.AuthorId, opt => opt.Ignore())
            .ForMember(dest => dest.UpvoteCount, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.DownvoteCount, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.CommentCount, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.ViewCount, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.IsLocked, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.Author, opt => opt.Ignore())
            .ForMember(dest => dest.Group, opt => opt.Ignore())
            .ForMember(dest => dest.Media, opt => opt.Ignore())
            .ForMember(dest => dest.Reactions, opt => opt.Ignore())
            .ForMember(dest => dest.Comments, opt => opt.Ignore())
            .ForMember(dest => dest.Tags, opt => opt.Ignore())
            .ForMember(dest => dest.Reports, opt => opt.Ignore());

        CreateMap<UpdatePostDto, Post>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Title, opt => opt.Ignore())
            .ForMember(dest => dest.Type, opt => opt.Ignore())
            .ForMember(dest => dest.Url, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.AuthorId, opt => opt.Ignore())
            .ForMember(dest => dest.GroupId, opt => opt.Ignore())
            .ForMember(dest => dest.UpvoteCount, opt => opt.Ignore())
            .ForMember(dest => dest.DownvoteCount, opt => opt.Ignore())
            .ForMember(dest => dest.CommentCount, opt => opt.Ignore())
            .ForMember(dest => dest.ViewCount, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.IsLocked, opt => opt.Ignore())
            .ForMember(dest => dest.Author, opt => opt.Ignore())
            .ForMember(dest => dest.Group, opt => opt.Ignore())
            .ForMember(dest => dest.Media, opt => opt.Ignore())
            .ForMember(dest => dest.Reactions, opt => opt.Ignore())
            .ForMember(dest => dest.Comments, opt => opt.Ignore())
            .ForMember(dest => dest.Tags, opt => opt.Ignore())
            .ForMember(dest => dest.Reports, opt => opt.Ignore());

        CreateMap<PostMedia, PostMediaDto>();

        CreateMap<PostComment, PostCommentDto>()
            .ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.GetScore()))
            .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.UserName))
            .ForMember(dest => dest.AuthorProfileImage, opt => opt.MapFrom(src => src.Author.ProfileImageUrl))
            .ForMember(dest => dest.Replies, opt => opt.MapFrom(src => src.Replies))
            .ForMember(dest => dest.CurrentUserReaction, opt => opt.Ignore());

        CreateMap<CreatePostCommentDto, PostComment>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.AuthorId, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.IsEdited, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.UpvoteCount, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.DownvoteCount, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.ReplyCount, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.Post, opt => opt.Ignore())
            .ForMember(dest => dest.Author, opt => opt.Ignore())
            .ForMember(dest => dest.ParentComment, opt => opt.Ignore())
            .ForMember(dest => dest.Replies, opt => opt.Ignore())
            .ForMember(dest => dest.Reactions, opt => opt.Ignore())
            .ForMember(dest => dest.Reports, opt => opt.Ignore());

        CreateMap<UpdatePostCommentDto, PostComment>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PostId, opt => opt.Ignore())
            .ForMember(dest => dest.AuthorId, opt => opt.Ignore())
            .ForMember(dest => dest.ParentCommentId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.IsEdited, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.UpvoteCount, opt => opt.Ignore())
            .ForMember(dest => dest.DownvoteCount, opt => opt.Ignore())
            .ForMember(dest => dest.ReplyCount, opt => opt.Ignore())
            .ForMember(dest => dest.Post, opt => opt.Ignore())
            .ForMember(dest => dest.Author, opt => opt.Ignore())
            .ForMember(dest => dest.ParentComment, opt => opt.Ignore())
            .ForMember(dest => dest.Replies, opt => opt.Ignore())
            .ForMember(dest => dest.Reactions, opt => opt.Ignore())
            .ForMember(dest => dest.Reports, opt => opt.Ignore());
    }
}
'@
Set-Content -Path "CineSocial.Core.Application\Mapping\PostMappingProfile.cs" -Value $postMappingContent

# ================================
# 6. CREATE SERVICE IMPLEMENTATIONS
# ================================

Write-Host "Creating Service Implementations..." -ForegroundColor Yellow

# Group Service Implementation
Write-Host "Creating GroupService..." -ForegroundColor Cyan
$groupServiceImplContent = @'
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using CineSocial.Core.Application.Ports;
using CineSocial.Core.Application.DTOs.Common;
using CineSocial.Core.Application.DTOs.Groups;
using CineSocial.Core.Domain.Entities;
using CineSocial.Adapters.Infrastructure.Database;

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
                query = query.Where(g => g.Name.Contains(search) || g.Description.Contains(search));
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
                groupDto.CurrentUserRole = membership?.Role;

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

            // Add creator as owner
            var membership = new GroupMember
            {
                Id = Guid.NewGuid(),
                GroupId = group.Id,
                UserId = userId,
                Role = GroupRole.Owner,
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
            if (membership == null || (membership.Role != GroupRole.Owner && membership.Role != GroupRole.Admin))
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
            if (membership == null || membership.Role != GroupRole.Owner)
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
                    Role = GroupRole.Member,
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

            if (membership.Role == GroupRole.Owner)
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

    public async Task<Result> UpdateMemberRoleAsync(Guid userId, Guid groupId, Guid memberId, GroupRole newRole)
    {
        try
        {
            var userMembership = await _context.GroupMembers
                .FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == userId && m.IsActive);

            if (userMembership == null || (userMembership.Role != GroupRole.Owner && userMembership.Role != GroupRole.Admin))
            {
                return Result.Failure("Insufficient permissions");
            }

            var targetMembership = await _context.GroupMembers
                .FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == memberId && m.IsActive);

            if (targetMembership == null)
            {
                return Result.Failure("Member not found");
            }

            if (newRole == GroupRole.Owner)
            {
                return Result.Failure("Cannot assign owner role");
            }

            targetMembership.Role = newRole;
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

            if (userMembership == null || (userMembership.Role != GroupRole.Owner && userMembership.Role != GroupRole.Admin && userMembership.Role != GroupRole.Moderator))
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

            if (userMembership == null || (userMembership.Role != GroupRole.Owner && userMembership.Role != GroupRole.Admin && userMembership.Role != GroupRole.Moderator))
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
'@
Set-Content -Path "CineSocial.Adapters.Infrastructure\Services\GroupService.cs" -Value $groupServiceImplContent

# ================================
# 7. UPDATE DATABASE CONTEXT
# ================================

Write-Host "Updating Database Context..." -ForegroundColor Yellow
$dbContextUpdateContent = @'

        // Reddit-like platform DbSets (Add these to ApplicationDbContext)
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostMedia> PostMedias { get; set; }
        public DbSet<PostComment> PostComments { get; set; }
        public DbSet<PostReaction> PostReactions { get; set; }
        public DbSet<CommentReaction> CommentReactions { get; set; }
        public DbSet<GroupBan> GroupBans { get; set; }
        public DbSet<UserBlock> UserBlocks { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<PostTag> PostTags { get; set; }
        public DbSet<Following> Followings { get; set; }

        // Add these to OnModelCreating method:
        builder.Entity<Group>(entity =>
        {
            entity.ToTable("Groups");
            entity.Property(e => e.Id).HasColumnType("uuid");
            entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Rules).HasMaxLength(5000);
            entity.Property(e => e.IconUrl).HasMaxLength(500);
            entity.Property(e => e.BannerUrl).HasMaxLength(500);
            entity.Property(e => e.CreatedById).HasColumnType("uuid");
            entity.HasIndex(e => e.Name).IsUnique();
            
            entity.HasOne(e => e.CreatedBy)
                .WithMany(u => u.CreatedGroups)
                .HasForeignKey(e => e.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<GroupMember>(entity =>
        {
            entity.ToTable("GroupMembers");
            entity.Property(e => e.Id).HasColumnType("uuid");
            entity.Property(e => e.GroupId).HasColumnType("uuid");
            entity.Property(e => e.UserId).HasColumnType("uuid");
            entity.HasIndex(e => new { e.GroupId, e.UserId }).IsUnique();

            entity.HasOne(e => e.Group)
                .WithMany(g => g.Members)
                .HasForeignKey(e => e.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany(u => u.GroupMemberships)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Post>(entity =>
        {
            entity.ToTable("Posts");
            entity.Property(e => e.Id).HasColumnType("uuid");
            entity.Property(e => e.AuthorId).HasColumnType("uuid");
            entity.Property(e => e.GroupId).HasColumnType("uuid");
            entity.Property(e => e.Title).HasMaxLength(300).IsRequired();
            entity.Property(e => e.Content).HasMaxLength(10000);
            entity.Property(e => e.Url).HasMaxLength(1000);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.GroupId);
            entity.HasIndex(e => e.AuthorId);

            entity.HasOne(e => e.Author)
                .WithMany(u => u.Posts)
                .HasForeignKey(e => e.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Group)
                .WithMany(g => g.Posts)
                .HasForeignKey(e => e.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<PostMedia>(entity =>
        {
            entity.ToTable("PostMedias");
            entity.Property(e => e.Id).HasColumnType("uuid");
            entity.Property(e => e.PostId).HasColumnType("uuid");
            entity.Property(e => e.FileName).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Url).HasMaxLength(1000).IsRequired();
            entity.Property(e => e.ThumbnailUrl).HasMaxLength(1000);
            entity.Property(e => e.MimeType).HasMaxLength(100);

            entity.HasOne(e => e.Post)
                .WithMany(p => p.Media)
                .HasForeignKey(e => e.PostId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<PostComment>(entity =>
        {
            entity.ToTable("PostComments");
            entity.Property(e => e.Id).HasColumnType("uuid");
            entity.Property(e => e.PostId).HasColumnType("uuid");
            entity.Property(e => e.AuthorId).HasColumnType("uuid");
            entity.Property(e => e.ParentCommentId).HasColumnType("uuid");
            entity.Property(e => e.Content).HasMaxLength(2000).IsRequired();
            entity.HasIndex(e => e.PostId);
            entity.HasIndex(e => e.AuthorId);
            entity.HasIndex(e => e.ParentCommentId);

            entity.HasOne(e => e.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(e => e.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Author)
                .WithMany(u => u.PostComments)
                .HasForeignKey(e => e.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.ParentComment)
                .WithMany(c => c.Replies)
                .HasForeignKey(e => e.ParentCommentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<PostReaction>(entity =>
        {
            entity.ToTable("PostReactions");
            entity.Property(e => e.Id).HasColumnType("uuid");
            entity.Property(e => e.PostId).HasColumnType("uuid");
            entity.Property(e => e.UserId).HasColumnType("uuid");
            entity.HasIndex(e => new { e.PostId, e.UserId }).IsUnique();

            entity.HasOne(e => e.Post)
                .WithMany(p => p.Reactions)
                .HasForeignKey(e => e.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany(u => u.PostReactions)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<CommentReaction>(entity =>
        {
            entity.ToTable("CommentReactions");
            entity.Property(e => e.Id).HasColumnType("uuid");
            entity.Property(e => e.CommentId).HasColumnType("uuid");
            entity.Property(e => e.UserId).HasColumnType("uuid");
            entity.HasIndex(e => new { e.CommentId, e.UserId }).IsUnique();

            entity.HasOne(e => e.Comment)
                .WithMany(c => c.Reactions)
                .HasForeignKey(e => e.CommentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany(u => u.CommentReactions)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<GroupBan>(entity =>
        {
            entity.ToTable("GroupBans");
            entity.Property(e => e.Id).HasColumnType("uuid");
            entity.Property(e => e.GroupId).HasColumnType("uuid");
            entity.Property(e => e.UserId).HasColumnType("uuid");
            entity.Property(e => e.BannedById).HasColumnType("uuid");
            entity.Property(e => e.Reason).HasMaxLength(500);
            entity.HasIndex(e => new { e.GroupId, e.UserId });

            entity.HasOne(e => e.Group)
                .WithMany(g => g.Bans)
                .HasForeignKey(e => e.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany(u => u.GroupBans)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.BannedBy)
                .WithMany(u => u.IssuedBans)
                .HasForeignKey(e => e.BannedById)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<UserBlock>(entity =>
        {
            entity.ToTable("UserBlocks");
            entity.Property(e => e.Id).HasColumnType("uuid");
            entity.Property(e => e.BlockerId).HasColumnType("uuid");
            entity.Property(e => e.BlockedId).HasColumnType("uuid");
            entity.HasIndex(e => new { e.BlockerId, e.BlockedId }).IsUnique();

            entity.HasOne(e => e.Blocker)
                .WithMany(u => u.BlockedUsers)
                .HasForeignKey(e => e.BlockerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Blocked)
                .WithMany(u => u.BlockedByUsers)
                .HasForeignKey(e => e.BlockedId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Report>(entity =>
        {
            entity.ToTable("Reports");
            entity.Property(e => e.Id).HasColumnType("uuid");
            entity.Property(e => e.ReporterId).HasColumnType("uuid");
            entity.Property(e => e.TargetId).HasColumnType("uuid");
            entity.Property(e => e.ReviewedById).HasColumnType("uuid");
            entity.Property(e => e.Details).HasMaxLength(1000);
            entity.Property(e => e.ReviewNotes).HasMaxLength(1000);

            entity.HasOne(e => e.Reporter)
                .WithMany(u => u.Reports)
                .HasForeignKey(e => e.ReporterId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.ReviewedBy)
                .WithMany(u => u.ReviewedReports)
                .HasForeignKey(e => e.ReviewedById)
                .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<PostTag>(entity =>
        {
            entity.ToTable("PostTags");
            entity.Property(e => e.Id).HasColumnType("uuid");
            entity.Property(e => e.PostId).HasColumnType("uuid");
            entity.Property(e => e.Tag).HasMaxLength(50).IsRequired();
            entity.HasIndex(e => e.Tag);

            entity.HasOne(e => e.Post)
                .WithMany(p => p.Tags)
                .HasForeignKey(e => e.PostId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Following>(entity =>
        {
            entity.ToTable("Followings");
            entity.Property(e => e.Id).HasColumnType("uuid");
            entity.Property(e => e.FollowerId).HasColumnType("uuid");
            entity.Property(e => e.FollowingId).HasColumnType("uuid");
            entity.HasIndex(e => new { e.FollowerId, e.FollowingId }).IsUnique();

            entity.HasOne(e => e.Follower)
                .WithMany(u => u.Followings)
                .HasForeignKey(e => e.FollowerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Following)
                .WithMany(u => u.Followers)
                .HasForeignKey(e => e.FollowingId)
                .OnDelete(DeleteBehavior.Restrict);
        });
'@

Write-Host "Creating Database Context update instructions..." -ForegroundColor Cyan
Add-Content -Path "database_context_updates.txt" -Value $dbContextUpdateContent

# ================================
# 8. CREATE CONTROLLERS
# ================================

Write-Host "Creating Controllers..." -ForegroundColor Yellow

# Groups Controller
Write-Host "Creating GroupsController..." -ForegroundColor Cyan
$groupsControllerContent = @'
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
'@
Set-Content -Path "CineSocial.Adapters.WebAPI\Controllers\GroupsController.cs" -Value $groupsControllerContent

# Posts Controller
Write-Host "Creating PostsController..." -ForegroundColor Cyan
$postsControllerContent = @'
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
'@
Set-Content -Path "CineSocial.Adapters.WebAPI\Controllers\PostsController.cs" -Value $postsControllerContent

# ================================
# 9. CREATE POST SERVICE IMPLEMENTATION
# ================================

Write-Host "Creating PostService Implementation..." -ForegroundColor Yellow
$postServiceImplContent = @'
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using CineSocial.Core.Application.Ports;
using CineSocial.Core.Application.DTOs.Common;
using CineSocial.Core.Application.DTOs.Posts;
using CineSocial.Core.Domain.Entities;
using CineSocial.Adapters.Infrastructure.Database;

namespace CineSocial.Adapters.Infrastructure.Services;

public class PostService : IPostService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public PostService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<PostSummaryDto>>> GetPostsAsync(int page = 1, int pageSize = 20, Guid? groupId = null, Guid? userId = null, string? search = null, string? sortBy = null)
    {
        try
        {
            var query = _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Group)
                .Include(p => p.Media)
                .Where(p => !p.IsDeleted)
                .AsQueryable();

            if (groupId.HasValue)
                query = query.Where(p => p.GroupId == groupId.Value);
            
            if (userId.HasValue)
                query = query.Where(p => p.AuthorId == userId.Value);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(p => p.Title.Contains(search) || p.Content.Contains(search));

            query = sortBy?.ToLower() switch
            {
                "hot" => query.OrderByDescending(p => p.UpvoteCount - p.DownvoteCount).ThenByDescending(p => p.CreatedAt),
                "new" => query.OrderByDescending(p => p.CreatedAt),
                "top" => query.OrderByDescending(p => p.UpvoteCount),
                _ => query.OrderByDescending(p => (p.UpvoteCount - p.DownvoteCount) * 1.0 / Math.Max(1, (DateTime.UtcNow - p.CreatedAt).TotalHours))
            };

            var totalCount = await query.CountAsync();
            var posts = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var postDtos = _mapper.Map<List<PostSummaryDto>>(posts);
            var result = new PagedResult<PostSummaryDto>(postDtos, totalCount, page, pageSize);
            return Result<PagedResult<PostSummaryDto>>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<PagedResult<PostSummaryDto>>.Failure($"Error getting posts: {ex.Message}");
        }
    }

    public async Task<Result<PostDto>> GetPostByIdAsync(Guid id, Guid? currentUserId = null)
    {
        try
        {
            var post = await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Group)
                .Include(p => p.Media)
                .Include(p => p.Tags)
                .Include(p => p.Reactions)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

            if (post == null)
                return Result<PostDto>.Failure("Post not found");

            // Increment view count
            post.ViewCount++;
            await _context.SaveChangesAsync();

            var postDto = _mapper.Map<PostDto>(post);

            if (currentUserId.HasValue)
            {
                var userReaction = post.Reactions.FirstOrDefault(r => r.UserId == currentUserId.Value);
                postDto.CurrentUserReaction = userReaction?.Type;
            }

            return Result<PostDto>.Success(postDto);
        }
        catch (Exception ex)
        {
            return Result<PostDto>.Failure($"Error getting post: {ex.Message}");
        }
    }

    public async Task<Result<PostDto>> CreatePostAsync(Guid userId, CreatePostDto createDto)
    {
        try
        {
            // Check if user is member of the group
            var isMember = await _context.GroupMembers
                .AnyAsync(m => m.GroupId == createDto.GroupId && m.UserId == userId && m.IsActive);

            if (!isMember)
                return Result<PostDto>.Failure("You must be a member of the group to post");

            var post = _mapper.Map<Post>(createDto);
            post.Id = Guid.NewGuid();
            post.AuthorId = userId;
            post.CreatedAt = DateTime.UtcNow;

            _context.Posts.Add(post);

            // Add tags
            foreach (var tag in createDto.Tags)
            {
                var postTag = new PostTag
                {
                    Id = Guid.NewGuid(),
                    PostId = post.Id,
                    Tag = tag.Trim().ToLower(),
                    CreatedAt = DateTime.UtcNow
                };
                _context.PostTags.Add(postTag);
            }

            await _context.SaveChangesAsync();
            return await GetPostByIdAsync(post.Id, userId);
        }
        catch (Exception ex)
        {
            return Result<PostDto>.Failure($"Error creating post: {ex.Message}");
        }
    }

    public async Task<Result<PostDto>> UpdatePostAsync(Guid userId, Guid postId, UpdatePostDto updateDto)
    {
        try
        {
            var post = await _context.Posts
                .Include(p => p.Tags)
                .FirstOrDefaultAsync(p => p.Id == postId && p.AuthorId == userId && !p.IsDeleted);

            if (post == null)
                return Result<PostDto>.Failure("Post not found");

            _mapper.Map(updateDto, post);
            post.UpdatedAt = DateTime.UtcNow;

            // Update tags
            _context.PostTags.RemoveRange(post.Tags);
            foreach (var tag in updateDto.Tags)
            {
                var postTag = new PostTag
                {
                    Id = Guid.NewGuid(),
                    PostId = post.Id,
                    Tag = tag.Trim().ToLower(),
                    CreatedAt = DateTime.UtcNow
                };
                _context.PostTags.Add(postTag);
            }

            await _context.SaveChangesAsync();
            return await GetPostByIdAsync(postId, userId);
        }
        catch (Exception ex)
        {
            return Result<PostDto>.Failure($"Error updating post: {ex.Message}");
        }
    }

    public async Task<r> DeletePostAsync(Guid userId, Guid postId)
    {
        try
        {
            var post = await _context.Posts
                .FirstOrDefaultAsync(p => p.Id == postId && p.AuthorId == userId);

            if (post == null)
                return Result.Failure("Post not found");

            post.IsDeleted = true;
            post.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error deleting post: {ex.Message}");
        }
    }

    public async Task<r> ReactToPostAsync(Guid userId, Guid postId, ReactionType reactionType)
    {
        try
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post == null)
                return Result.Failure("Post not found");

            var existingReaction = await _context.PostReactions
                .FirstOrDefaultAsync(r => r.PostId == postId && r.UserId == userId);

            if (existingReaction != null)
            {
                if (existingReaction.Type == reactionType)
                    return Result.Failure("You already reacted with this type");

                // Update counts
                if (existingReaction.Type == ReactionType.Upvote)
                    post.UpvoteCount--;
                else
                    post.DownvoteCount--;

                existingReaction.Type = reactionType;
            }
            else
            {
                var reaction = new PostReaction
                {
                    Id = Guid.NewGuid(),
                    PostId = postId,
                    UserId = userId,
                    Type = reactionType,
                    CreatedAt = DateTime.UtcNow
                };
                _context.PostReactions.Add(reaction);
            }

            // Update counts
            if (reactionType == ReactionType.Upvote)
                post.UpvoteCount++;
            else
                post.DownvoteCount++;

            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error reacting to post: {ex.Message}");
        }
    }

    public async Task<r> RemovePostReactionAsync(Guid userId, Guid postId)
    {
        try
        {
            var reaction = await _context.PostReactions
                .FirstOrDefaultAsync(r => r.PostId == postId && r.UserId == userId);

            if (reaction == null)
                return Result.Failure("No reaction found");

            var post = await _context.Posts.FindAsync(postId);
            if (post != null)
            {
                if (reaction.Type == ReactionType.Upvote)
                    post.UpvoteCount--;
                else
                    post.DownvoteCount--;
            }

            _context.PostReactions.Remove(reaction);
            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error removing reaction: {ex.Message}");
        }
    }

    public async Task<Result<PagedResult<PostCommentDto>>> GetPostCommentsAsync(Guid postId, int page = 1, int pageSize = 20, Guid? currentUserId = null)
    {
        try
        {
            var query = _context.PostComments
                .Include(c => c.Author)
                .Include(c => c.Replies)
                .ThenInclude(r => r.Author)
                .Where(c => c.PostId == postId && c.ParentCommentId == null && !c.IsDeleted)
                .OrderByDescending(c => c.UpvoteCount - c.DownvoteCount)
                .ThenBy(c => c.CreatedAt);

            var totalCount = await query.CountAsync();
            var comments = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var commentDtos = _mapper.Map<List<PostCommentDto>>(comments);

            if (currentUserId.HasValue)
            {
                // Set current user reactions for comments and replies
                var allCommentIds = comments.SelectMany(c => new[] { c.Id }.Concat(c.Replies.Select(r => r.Id))).ToList();
                var userReactions = await _context.CommentReactions
                    .Where(r => allCommentIds.Contains(r.CommentId) && r.UserId == currentUserId.Value)
                    .ToListAsync();

                foreach (var commentDto in commentDtos)
                {
                    var reaction = userReactions.FirstOrDefault(r => r.CommentId == commentDto.Id);
                    commentDto.CurrentUserReaction = reaction?.Type;

                    foreach (var reply in commentDto.Replies)
                    {
                        var replyReaction = userReactions.FirstOrDefault(r => r.CommentId == reply.Id);
                        reply.CurrentUserReaction = replyReaction?.Type;
                    }
                }
            }

            var result = new PagedResult<PostCommentDto>(commentDtos, totalCount, page, pageSize);
            return Result<PagedResult<PostCommentDto>>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<PagedResult<PostCommentDto>>.Failure($"Error getting comments: {ex.Message}");
        }
    }

    public async Task<Result<PostCommentDto>> CreateCommentAsync(Guid userId, CreatePostCommentDto createDto)
    {
        try
        {
            var comment = _mapper.Map<PostComment>(createDto);
            comment.Id = Guid.NewGuid();
            comment.AuthorId = userId;
            comment.CreatedAt = DateTime.UtcNow;

            _context.PostComments.Add(comment);

            // Update post comment count
            var post = await _context.Posts.FindAsync(createDto.PostId);
            if (post != null)
            {
                post.CommentCount++;
            }

            // Update parent comment reply count
            if (createDto.ParentCommentId.HasValue)
            {
                var parentComment = await _context.PostComments.FindAsync(createDto.ParentCommentId.Value);
                if (parentComment != null)
                {
                    parentComment.ReplyCount++;
                }
            }

            await _context.SaveChangesAsync();

            var createdComment = await _context.PostComments
                .Include(c => c.Author)
                .Include(c => c.Replies)
                .ThenInclude(r => r.Author)
                .FirstOrDefaultAsync(c => c.Id == comment.Id);

            var commentDto = _mapper.Map<PostCommentDto>(createdComment);
            return Result<PostCommentDto>.Success(commentDto);
        }
        catch (Exception ex)
        {
            return Result<PostCommentDto>.Failure($"Error creating comment: {ex.Message}");
        }
    }

    public async Task<Result<PostCommentDto>> UpdateCommentAsync(Guid userId, Guid commentId, UpdatePostCommentDto updateDto)
    {
        try
        {
            var comment = await _context.PostComments
                .FirstOrDefaultAsync(c => c.Id == commentId && c.AuthorId == userId && !c.IsDeleted);

            if (comment == null)
                return Result<PostCommentDto>.Failure("Comment not found");

            _mapper.Map(updateDto, comment);
            comment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var updatedComment = await _context.PostComments
                .Include(c => c.Author)
                .Include(c => c.Replies)
                .ThenInclude(r => r.Author)
                .FirstOrDefaultAsync(c => c.Id == commentId);

            var commentDto = _mapper.Map<PostCommentDto>(updatedComment);
            return Result<PostCommentDto>.Success(commentDto);
        }
        catch (Exception ex)
        {
            return Result<PostCommentDto>.Failure($"Error updating comment: {ex.Message}");
        }
    }

    public async Task<r> DeleteCommentAsync(Guid userId, Guid commentId)
    {
        try
        {
            var comment = await _context.PostComments
                .FirstOrDefaultAsync(c => c.Id == commentId && c.AuthorId == userId);

            if (comment == null)
                return Result.Failure("Comment not found");

            comment.IsDeleted = true;
            comment.UpdatedAt = DateTime.UtcNow;

            // Update post comment count
            var post = await _context.Posts.FindAsync(comment.PostId);
            if (post != null)
            {
                post.CommentCount--;
            }

            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error deleting comment: {ex.Message}");
        }
    }

    public async Task<r> ReactToCommentAsync(Guid userId, Guid commentId, ReactionType reactionType)
    {
        try
        {
            var comment = await _context.PostComments.FindAsync(commentId);
            if (comment == null)
                return Result.Failure("Comment not found");

            var existingReaction = await _context.CommentReactions
                .FirstOrDefaultAsync(r => r.CommentId == commentId && r.UserId == userId);

            if (existingReaction != null)
            {
                if (existingReaction.Type == reactionType)
                    return Result.Failure("You already reacted with this type");

                // Update counts
                if (existingReaction.Type == ReactionType.Upvote)
                    comment.UpvoteCount--;
                else
                    comment.DownvoteCount--;

                existingReaction.Type = reactionType;
            }
            else
            {
                var reaction = new CommentReaction
                {
                    Id = Guid.NewGuid(),
                    CommentId = commentId,
                    UserId = userId,
                    Type = reactionType,
                    CreatedAt = DateTime.UtcNow
                };
                _context.CommentReactions.Add(reaction);
            }

            // Update counts
            if (reactionType == ReactionType.Upvote)
                comment.UpvoteCount++;
            else
                comment.DownvoteCount++;

            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error reacting to comment: {ex.Message}");
        }
    }

    public async Task<r> RemoveCommentReactionAsync(Guid userId, Guid commentId)
    {
        try
        {
            var reaction = await _context.CommentReactions
                .FirstOrDefaultAsync(r => r.CommentId == commentId && r.UserId == userId);

            if (reaction == null)
                return Result.Failure("No reaction found");

            var comment = await _context.PostComments.FindAsync(commentId);
            if (comment != null)
            {
                if (reaction.Type == ReactionType.Upvote)
                    comment.UpvoteCount--;
                else
                    comment.DownvoteCount--;
            }

            _context.CommentReactions.Remove(reaction);
            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error removing comment reaction: {ex.Message}");
        }
    }

    public async Task<Result<List<PostSummaryDto>>> GetTrendingPostsAsync(int count = 10)
    {
        try
        {
            var posts = await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Group)
                .Include(p => p.Media)
                .Where(p => !p.IsDeleted && p.CreatedAt >= DateTime.UtcNow.AddDays(-7))
                .OrderByDescending(p => (p.UpvoteCount - p.DownvoteCount) * 1.0 / Math.Max(1, (DateTime.UtcNow - p.CreatedAt).TotalHours))
                .Take(count)
                .ToListAsync();

            var postDtos = _mapper.Map<List<PostSummaryDto>>(posts);
            return Result<List<PostSummaryDto>>.Success(postDtos);
        }
        catch (Exception ex)
        {
            return Result<List<PostSummaryDto>>.Failure($"Error getting trending posts: {ex.Message}");
        }
    }

    public async Task<Result<List<PostSummaryDto>>> GetUserFeedAsync(Guid userId, int page = 1, int pageSize = 20)
    {
        try
        {
            // Get posts from groups user is member of
            var memberGroupIds = await _context.GroupMembers
                .Where(m => m.UserId == userId && m.IsActive)
                .Select(m => m.GroupId)
                .ToListAsync();

            var posts = await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Group)
                .Include(p => p.Media)
                .Where(p => !p.IsDeleted && memberGroupIds.Contains(p.GroupId))
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var postDtos = _mapper.Map<List<PostSummaryDto>>(posts);
            return Result<List<PostSummaryDto>>.Success(postDtos);
        }
        catch (Exception ex)
        {
            return Result<List<PostSummaryDto>>.Failure($"Error getting user feed: {ex.Message}");
        }
    }
}
'@
Set-Content -Path "CineSocial.Adapters.Infrastructure\Services\PostService.cs" -Value $postServiceImplContent

# ================================
# 10. UPDATE PROGRAM.CS
# ================================

Write-Host "Creating Program.cs update instructions..." -ForegroundColor Yellow
$programUpdateContent = @'

// Add these service registrations to Program.cs:

builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IPostService, PostService>();

// Add these after the existing AutoMapper registration:
builder.Services.AddAutoMapper(typeof(GroupMappingProfile));
builder.Services.AddAutoMapper(typeof(PostMappingProfile));

'@
Add-Content -Path "program_cs_updates.txt" -Value $programUpdateContent

# ================================
# 11. FIX SERVICE INTERFACE TYPOS
# ================================

Write-Host "Fixing service interface typos..." -ForegroundColor Yellow

# Fix IGroupService
$groupServicePath = "CineSocial.Core.Application\Ports\IGroupService.cs"
if (Test-Path $groupServicePath) {
    $groupServiceContent = Get-Content $groupServicePath -Raw
    $groupServiceContent = $groupServiceContent -replace 'Task<r>', 'Task<Result>'
    Set-Content -Path $groupServicePath -Value $groupServiceContent
}

# Fix IPostService
$postServicePath = "CineSocial.Core.Application\Ports\IPostService.cs"
if (Test-Path $postServicePath) {
    $postServiceContent = Get-Content $postServicePath -Raw
    $postServiceContent = $postServiceContent -replace 'Task<r>', 'Task<Result>'
    Set-Content -Path $postServicePath -Value $postServiceContent
}

# Fix GroupService Implementation
$groupServiceImplPath = "CineSocial.Adapters.Infrastructure\Services\GroupService.cs"
if (Test-Path $groupServiceImplPath) {
    $groupServiceImplContent = Get-Content $groupServiceImplPath -Raw
    $groupServiceImplContent = $groupServiceImplContent -replace 'Task<r>', 'Task<Result>'
    Set-Content -Path $groupServiceImplPath -Value $groupServiceImplContent
}

# Fix PostService Implementation
$postServiceImplPath = "CineSocial.Adapters.Infrastructure\Services\PostService.cs"
if (Test-Path $postServiceImplPath) {
    $postServiceImplContent = Get-Content $postServiceImplPath -Raw
    $postServiceImplContent = $postServiceImplContent -replace 'Task<r>', 'Task<Result>'
    Set-Content -Path $postServiceImplPath -Value $postServiceImplContent
}

# ================================
# 12. CREATE MIGRATION INSTRUCTIONS
# ================================

Write-Host "Creating migration instructions..." -ForegroundColor Green
$migrationInstructions = @'

MIGRATION INSTRUCTIONS:
=======================

1. Update ApplicationDbContext.cs:
   - Add the new DbSet properties listed in database_context_updates.txt
   - Add the entity configurations to OnModelCreating method

2. Update User.cs entity:
   - Replace the existing User entity with the updated version created by this script

3. Run EF Core migration:
   dotnet ef migrations add AddRedditPlatformEntities --project CineSocial.Adapters.Infrastructure --startup-project CineSocial.Adapters.WebAPI
   dotnet ef database update --project CineSocial.Adapters.Infrastructure --startup-project CineSocial.Adapters.WebAPI

4. Update Program.cs:
   - Add the service registrations listed in program_cs_updates.txt

5. Test the endpoints:
   - GET /api/groups - List groups
   - POST /api/groups - Create group
   - GET /api/groups/{id} - Get group details
   - POST /api/groups/{id}/join - Join group
   - GET /api/posts - List posts
   - POST /api/posts - Create post
   - GET /api/posts/{id} - Get post details
   - GET /api/posts/{id}/comments - Get post comments

FILES CREATED:
==============
- Domain Entities: Group, GroupMember, Post, PostMedia, PostComment, PostReaction, CommentReaction, GroupBan, UserBlock, Report, PostTag, Following
- DTOs: GroupDtos.cs, PostDtos.cs
- Service Interfaces: IGroupService.cs, IPostService.cs
- Service Implementations: GroupService.cs, PostService.cs
- Mapping Profiles: GroupMappingProfile.cs, PostMappingProfile.cs
- Controllers: GroupsController.cs, PostsController.cs
- Updated User.cs entity

NEXT STEPS:
===========
1. Apply the database context changes
2. Run migrations
3. Update Program.cs with service registrations
4. Test the API endpoints
5. Add authentication/authorization where needed
6. Implement file upload for post media
7. Add notification system
8. Add search functionality
9. Add moderation tools

'@
Set-Content -Path "README_REDDIT_PLATFORM.md" -Value $migrationInstructions

Write-Host "Reddit-like Platform Entity Creation Complete!" -ForegroundColor Green
Write-Host "Check README_REDDIT_PLATFORM.md for next steps" -ForegroundColor Yellow