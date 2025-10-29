using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Social;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.Features.Comments.Queries.GetUserComments;

public class GetUserCommentsQueryHandler : IRequestHandler<GetUserCommentsQuery, Result<object>>
{
    private readonly IRepository<Comment> _commentRepository;

    public GetUserCommentsQueryHandler(IRepository<Comment> commentRepository)
    {
        _commentRepository = commentRepository;
    }

    public async Task<Result<object>> Handle(GetUserCommentsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var comments = await _commentRepository.GetQueryable()
                .Where(c => c.UserId == request.UserId && c.DeletedAt == null)
                .Include(c => c.User)
                .Include(c => c.Reactions)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync(cancellationToken);

            return Result<object>.Success(comments);
        }
        catch (Exception ex)
        {
            return Result<object>.Failure($"Failed to retrieve user comments: {ex.Message}");
        }
    }
}
