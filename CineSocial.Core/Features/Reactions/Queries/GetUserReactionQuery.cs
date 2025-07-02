using CineSocial.Core.Features.Reactions.Commands;
using CineSocial.Core.Shared;
using CineSocial.Core.Shared.Interfaces;
using MediatR;

namespace CineSocial.Core.Features.Reactions.Queries;

public record GetUserReactionQuery(
    Guid UserId,
    Guid CommentId
) : IRequest<Result<ReactionResult?>>;

public class GetUserReactionQueryHandler : IRequestHandler<GetUserReactionQuery, Result<ReactionResult?>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUserReactionQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ReactionResult?>> Handle(GetUserReactionQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var reaction = await _unitOfWork.Reactions.FirstOrDefaultAsync(
                r => r.UserId == request.UserId && r.CommentId == request.CommentId,
                cancellationToken
            );

            if (reaction == null)
            {
                return Result<ReactionResult?>.Success(null);
            }

            var result = new ReactionResult(
                reaction.Id,
                reaction.UserId,
                reaction.CommentId,
                reaction.Type,
                reaction.CreatedAt,
                reaction.UpdatedAt
            );

            return Result<ReactionResult?>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<ReactionResult?>.Failure($"Tepki sorgulanırken hata oluştu: {ex.Message}");
        }
    }
}