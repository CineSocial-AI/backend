using CineSocial.Core.Features.Comments.Commands;
using CineSocial.Core.Shared;
using CineSocial.Core.Shared.Interfaces;
using MediatR;

namespace CineSocial.Core.Features.Comments.Queries;

public record GetCommentByIdQuery(Guid Id) : IRequest<Result<CommentResult?>>;

public class GetCommentByIdQueryHandler : IRequestHandler<GetCommentByIdQuery, Result<CommentResult?>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCommentByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CommentResult?>> Handle(GetCommentByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var comment = await _unitOfWork.Comments.FirstOrDefaultAsync(
                c => c.Id == request.Id,
                c => c.User!
            );

            if (comment == null)
            {
                return Result<CommentResult?>.Success(null);
            }

            var result = new CommentResult(
                comment.Id,
                comment.UserId,
                comment.ReviewId,
                comment.Content,
                comment.UpvotesCount,
                comment.DownvotesCount,
                comment.CreatedAt,
                comment.UpdatedAt,
                comment.ParentCommentId,
                $"{comment.User?.FirstName} {comment.User?.LastName}",
                comment.User?.Username ?? ""
            );

            return Result<CommentResult?>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<CommentResult?>.Failure($"Yorum sorgulanırken hata oluştu: {ex.Message}");
        }
    }
}