using CineSocial.Application.Common.Interfaces;
using CineSocial.Domain.Entities.Social;
using CineSocial.Domain.Enums;

namespace CineSocial.Application.UseCases.Comments;

public class GetMovieCommentsUseCase
{
    private readonly IApplicationDbContext _context;

    public GetMovieCommentsUseCase(IApplicationDbContext context)
    {
        _context = context;
    }

    public IQueryable<Comment> Execute(int movieId)
    {
        return _context.Comments
            .Where(c => c.CommentableType == CommentableType.Movie
                        && c.CommentableId == movieId
                        && c.ParentCommentId == null
                        && !c.IsDeleted)
            .OrderByDescending(c => c.CreatedAt);
    }
}
