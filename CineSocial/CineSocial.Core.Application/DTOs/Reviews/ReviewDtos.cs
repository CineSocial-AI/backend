using System.ComponentModel.DataAnnotations;

namespace CineSocial.Core.Application.DTOs.Reviews;

public class ReviewDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid MovieId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public decimal Rating { get; set; }
    public bool IsSpoiler { get; set; }
    public int LikesCount { get; set; }
    public int DislikesCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserFullName { get; set; } = string.Empty;
    public string? UserProfileImage { get; set; }
    public string MovieTitle { get; set; } = string.Empty;
    public string? MoviePoster { get; set; }
    public bool? CurrentUserLike { get; set; }
    public int CommentsCount { get; set; }
}

public class CreateReviewDto
{
    [Required]
    public Guid MovieId { get; set; }

    [Required]
    [MinLength(5)]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MinLength(10)]
    [MaxLength(5000)]
    public string Content { get; set; } = string.Empty;

    [Required]
    [Range(1, 10)]
    public decimal Rating { get; set; }

    public bool IsSpoiler { get; set; }
}

public class UpdateReviewDto
{
    [Required]
    [MinLength(5)]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MinLength(10)]
    [MaxLength(5000)]
    public string Content { get; set; } = string.Empty;

    [Required]
    [Range(1, 10)]
    public decimal Rating { get; set; }

    public bool IsSpoiler { get; set; }
}

public class CommentDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ReviewId { get; set; }
    public Guid? ParentCommentId { get; set; }
    public string Content { get; set; } = string.Empty;
    public int LikesCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserFullName { get; set; } = string.Empty;
    public string? UserProfileImage { get; set; }
    public bool? CurrentUserLike { get; set; }
    public List<CommentDto> Replies { get; set; } = new();
}

public class CreateCommentDto
{
    [Required]
    public Guid ReviewId { get; set; }

    public Guid? ParentCommentId { get; set; }

    [Required]
    [MinLength(1)]
    [MaxLength(1000)]
    public string Content { get; set; } = string.Empty;
}
public class UpdateCommentDto
{
    [Required]
    [MinLength(1)]
    [MaxLength(1000)]
    public string Content { get; set; } = string.Empty;
}
