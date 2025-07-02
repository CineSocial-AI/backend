using CineSocial.Core.Shared;
using CineSocial.Core.Shared.Interfaces;
using CineSocial.Domain.Entities;
using MediatR;

namespace CineSocial.Core.Features.Reviews.Commands;

public record CreateReviewCommand(
    Guid UserId,
    Guid MovieId,
    string Title,
    string Content,
    bool ContainsSpoilers = false
) : IRequest<Result<ReviewResult>>;

public record ReviewResult(
    Guid Id,
    Guid UserId,
    Guid MovieId,
    string Title,
    string Content,
    bool ContainsSpoilers,
    int LikesCount,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string UserFullName,
    string UserUsername
);

public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, Result<ReviewResult>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateReviewCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ReviewResult>> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if movie exists
            var movie = await _unitOfWork.Movies.GetByIdAsync(request.MovieId, cancellationToken);
            if (movie == null)
            {
                return Result<ReviewResult>.Failure("Film bulunamadı.");
            }

            // Check if user exists
            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                return Result<ReviewResult>.Failure("Kullanıcı bulunamadı.");
            }

            // Check if user already reviewed this movie
            var existingReview = await _unitOfWork.Reviews.FirstOrDefaultAsync(
                r => r.UserId == request.UserId && r.MovieId == request.MovieId,
                cancellationToken
            );

            if (existingReview != null)
            {
                return Result<ReviewResult>.Failure("Bu film için zaten bir değerlendirme yazmışsınız.");
            }

            var review = new Review
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                MovieId = request.MovieId,
                Title = request.Title,
                Content = request.Content,
                ContainsSpoilers = request.ContainsSpoilers,
                LikesCount = 0,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Reviews.AddAsync(review, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var result = new ReviewResult(
                review.Id,
                review.UserId,
                review.MovieId,
                review.Title,
                review.Content,
                review.ContainsSpoilers,
                review.LikesCount,
                review.CreatedAt,
                review.UpdatedAt,
                $"{user.FirstName} {user.LastName}",
                user.Username
            );

            return Result<ReviewResult>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<ReviewResult>.Failure($"Değerlendirme oluşturulurken hata oluştu: {ex.Message}");
        }
    }
}