using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Application.Features.Comments.Queries.GetMovieComments;
using CineSocial.Domain.Enums;
using MediatR;

namespace CineSocial.Application.Features.Comments.Queries.GetCommentReplies;

public class GetCommentRepliesQueryHandler : IRequestHandler<GetCommentRepliesQuery, Result<PagedResult<CommentDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetCommentRepliesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public Task<Result<PagedResult<CommentDto>>> Handle(GetCommentRepliesQuery request, CancellationToken cancellationToken)
    {
        var query = from comment in _context.Comments
                    join user in _context.Users on comment.UserId equals user.Id
                    where comment.ParentCommentId == request.CommentId
                          && !comment.IsDeleted
                    orderby comment.CreatedAt ascending
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

        var totalCount = query.Count();

        var replies = query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var pagedResult = new PagedResult<CommentDto>(replies, totalCount, request.PageNumber, request.PageSize);

        return Task.FromResult(Result<PagedResult<CommentDto>>.Success(pagedResult));
    }
}
