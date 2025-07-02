using CineSocial.Core.Shared;
using CineSocial.Core.Shared.Interfaces;
using MediatR;

namespace CineSocial.Core.Features.MovieLists.Commands;

public record DeleteMovieListCommand(
    Guid Id,
    Guid UserId
) : IRequest<Result<bool>>;

public class DeleteMovieListCommandHandler : IRequestHandler<DeleteMovieListCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteMovieListCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DeleteMovieListCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var movieList = await _unitOfWork.MovieLists.GetByIdAsync(request.Id, cancellationToken);
            if (movieList == null)
            {
                return Result<bool>.Failure("Liste bulunamadı.");
            }

            // Check if user is the owner of the list
            if (movieList.UserId != request.UserId)
            {
                return Result<bool>.Failure("Bu listeyi silme yetkiniz yok.");
            }

            // Remove all items from the list first
            var listItems = await _unitOfWork.MovieListItems.FindAsync(
                mli => mli.MovieListId == request.Id,
                cancellationToken
            );

            _unitOfWork.MovieListItems.RemoveRange(listItems);

            // Remove the list
            _unitOfWork.MovieLists.Remove(movieList);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Liste silinirken hata oluştu: {ex.Message}");
        }
    }
}