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
