using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Application.Features.Comments.Queries.GetMovieComments;
using CineSocial.Domain.Enums;
using MediatR;

namespace CineSocial.Application.Features.Comments.Queries.GetCommentById;

public class GetCommentByIdQueryHandler : IRequestHandler<GetCommentByIdQuery, Result<CommentDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCommentByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public Task<Result<CommentDto>> Handle(GetCommentByIdQuery request, CancellationToken cancellationToken)
    {
        var query = from comment in _context.Comments
                    join user in _context.Users on comment.UserId equals user.Id
                    where comment.Id == request.CommentId && !comment.IsDeleted
                    select new CommentDto
                    {
                        Id = comment.Id,
                        UserId = user.Id,
                        Username = user.Username,
                        ProfileImageId = user.ProfileImageId,
                        Content = comment.Content,
                        Depth = comment.Depth,
                        IsEdited = comment.IsEdited,
                        EditedAt = comment.EditedAt,
                        CreatedAt = comment.CreatedAt,
                        UpvoteCount = _context.Reactions.Count(r => r.CommentId == comment.Id && r.Type == ReactionType.Upvote),
                        DownvoteCount = _context.Reactions.Count(r => r.CommentId == comment.Id && r.Type == ReactionType.Downvote),
                        ReplyCount = _context.Comments.Count(c => c.ParentCommentId == comment.Id && !c.IsDeleted)
                    };

        var result = query.FirstOrDefault();

        if (result == null)
        {
            return Task.FromResult(Result<CommentDto>.Failure("Comment not found"));
        }

        return Task.FromResult(Result<CommentDto>.Success(result, "Comment retrieved successfully"));
    }
}
