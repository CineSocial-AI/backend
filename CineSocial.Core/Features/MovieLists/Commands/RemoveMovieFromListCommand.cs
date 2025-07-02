using CineSocial.Core.Shared;
using CineSocial.Core.Shared.Interfaces;
using MediatR;

namespace CineSocial.Core.Features.MovieLists.Commands;

public record RemoveMovieFromListCommand(
    Guid UserId,
    Guid MovieListId,
    Guid MovieId
) : IRequest<Result<bool>>;

public class RemoveMovieFromListCommandHandler : IRequestHandler<RemoveMovieFromListCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public RemoveMovieFromListCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(RemoveMovieFromListCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if movie list exists and user owns it
            var movieList = await _unitOfWork.MovieLists.GetByIdAsync(request.MovieListId, cancellationToken);
            if (movieList == null)
            {
                return Result<bool>.Failure("Liste bulunamadı.");
            }

            if (movieList.UserId != request.UserId)
            {
                return Result<bool>.Failure("Bu listeden film çıkarma yetkiniz yok.");
            }

            // Find the movie list item
            var movieListItem = await _unitOfWork.MovieListItems.FirstOrDefaultAsync(
                mli => mli.MovieListId == request.MovieListId && mli.MovieId == request.MovieId,
                cancellationToken
            );

            if (movieListItem == null)
            {
                return Result<bool>.Failure("Film bu listede bulunamadı.");
            }

            _unitOfWork.MovieListItems.Remove(movieListItem);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Film listeden çıkarılırken hata oluştu: {ex.Message}");
        }
    }
}