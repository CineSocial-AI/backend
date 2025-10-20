namespace CineSocial.Application.Features.Comments.Queries.GetMovieComments;

public class CommentDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public int? ProfileImageId { get; set; }
    public string Content { get; set; } = string.Empty;
    public int Depth { get; set; }
    public bool IsEdited { get; set; }
    public DateTime? EditedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public int UpvoteCount { get; set; }
    public int DownvoteCount { get; set; }
    public int ReplyCount { get; set; }
}
