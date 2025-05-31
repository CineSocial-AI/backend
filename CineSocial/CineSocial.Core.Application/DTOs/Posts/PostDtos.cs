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
