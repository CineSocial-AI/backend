using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Domain.Enums;
using MediatR;

namespace CineSocial.Application.Features.Comments.Queries.GetMovieComments;

public class GetMovieCommentsQueryHandler : IRequestHandler<GetMovieCommentsQuery, Result<PagedResult<CommentDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetMovieCommentsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public Task<Result<PagedResult<CommentDto>>> Handle(GetMovieCommentsQuery request, CancellationToken cancellationToken)
    {
        var query = from comment in _context.Comments
                    join user in _context.Users on comment.UserId equals user.Id
                    where comment.CommentableType == CommentableType.Movie
                          && comment.CommentableId == request.MovieId
                          && comment.ParentCommentId == null
                          && !comment.IsDeleted
                    orderby comment.CreatedAt descending
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

        var comments = query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var pagedResult = new PagedResult<CommentDto>(comments, totalCount, request.PageNumber, request.PageSize);

        return Task.FromResult(Result<PagedResult<CommentDto>>.Success(pagedResult));
    }
}
