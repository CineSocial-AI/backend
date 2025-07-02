using System.ComponentModel.DataAnnotations;
using CineSocial.Domain.Enums;

namespace CineSocial.Api.DTOs;

public class ReviewDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid MovieId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool ContainsSpoilers { get; set; }
    public int LikesCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string UserFullName { get; set; } = string.Empty;
    public string UserUsername { get; set; } = string.Empty;
}

public class CreateReviewRequest
{
    [Required]
    public Guid MovieId { get; set; }

    [Required]
    [StringLength(200, MinimumLength = 5)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(2000, MinimumLength = 10)]
    public string Content { get; set; } = string.Empty;

    public bool ContainsSpoilers { get; set; } = false;
}

public class UpdateReviewRequest
{
    [Required]
    [StringLength(200, MinimumLength = 5)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(2000, MinimumLength = 10)]
    public string Content { get; set; } = string.Empty;

    public bool ContainsSpoilers { get; set; } = false;
}

public class CommentDto
{
    public Guid Id { get; set; }
    public Guid ReviewId { get; set; }
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int UpvotesCount { get; set; }
    public int DownvotesCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? ParentCommentId { get; set; }
    public List<CommentDto> Replies { get; set; } = new();
}

public class CreateCommentRequest
{
    [Required]
    public Guid ReviewId { get; set; }

    [Required]
    [StringLength(500, MinimumLength = 2)]
    public string Content { get; set; } = string.Empty;

    public Guid? ParentCommentId { get; set; }
}

public class UpdateCommentRequest
{
    [Required]
    [StringLength(500, MinimumLength = 2)]
    public string Content { get; set; } = string.Empty;
}

public class RatingDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid MovieId { get; set; }
    public int Score { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateRatingRequest
{
    [Required]
    public Guid MovieId { get; set; }

    [Required]
    [Range(1, 10)]
    public int Score { get; set; }
}

public class ReactionDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid CommentId { get; set; }
    public ReactionType Type { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateReactionRequest
{
    [Required]
    public Guid CommentId { get; set; }

    [Required]
    public ReactionType Type { get; set; }
}

public class FavoriteDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid MovieId { get; set; }
    public string MovieTitle { get; set; } = string.Empty;
    public string MoviePosterPath { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class AddToFavoritesRequest
{
    [Required]
    public Guid MovieId { get; set; }
}