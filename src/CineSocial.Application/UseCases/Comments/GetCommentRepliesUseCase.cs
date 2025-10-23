using CineSocial.Application.Common.Interfaces;
using CineSocial.Domain.Entities.Social;

namespace CineSocial.Application.UseCases.Comments;

public class GetCommentRepliesUseCase
{
    private readonly IApplicationDbContext _context;

    public GetCommentRepliesUseCase(IApplicationDbContext context)
    {
        _context = context;
    }

    public IQueryable<Comment> Execute(int commentId)
    {
        return _context.Comments
            .Where(c => c.ParentCommentId == commentId && !c.IsDeleted)
            .OrderByDescending(c => c.CreatedAt);
    }
}
