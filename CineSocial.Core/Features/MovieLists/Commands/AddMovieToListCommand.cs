using CineSocial.Core.Shared;
using CineSocial.Core.Shared.Interfaces;
using CineSocial.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Core.Features.MovieLists.Commands;

public record AddMovieToListCommand(
    Guid UserId,
    Guid MovieListId,
    Guid MovieId,
    string? Notes = null
) : IRequest<Result<MovieListItemResult>>;

public record MovieListItemResult(
    Guid Id,
    Guid MovieListId,
    Guid MovieId,
    string Notes,
    int Order,
    DateTime CreatedAt,
    string MovieTitle,
    string MoviePosterPath
);

public class AddMovieToListCommandHandler : IRequestHandler<AddMovieToListCommand, Result<MovieListItemResult>>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddMovieToListCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<MovieListItemResult>> Handle(AddMovieToListCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if movie list exists and user owns it
            var movieList = await _unitOfWork.MovieLists.GetByIdAsync(request.MovieListId, cancellationToken);
            if (movieList == null)
            {
                return Result<MovieListItemResult>.Failure("Liste bulunamadı.");
            }

            if (movieList.UserId != request.UserId)
            {
                return Result<MovieListItemResult>.Failure("Bu listeye film ekleme yetkiniz yok.");
            }

            // Check if movie exists
            var movie = await _unitOfWork.Movies.GetByIdAsync(request.MovieId, cancellationToken);
            if (movie == null)
            {
                return Result<MovieListItemResult>.Failure("Film bulunamadı.");
            }

            // Check if movie is already in the list
            var existingItem = await _unitOfWork.MovieListItems.FirstOrDefaultAsync(
                mli => mli.MovieListId == request.MovieListId && mli.MovieId == request.MovieId,
                cancellationToken
            );

            if (existingItem != null)
            {
                return Result<MovieListItemResult>.Failure("Bu film zaten listede mevcut.");
            }

            // Get the next order number
            var maxOrder = await _unitOfWork.MovieListItems
                .FindAsync(mli => mli.MovieListId == request.MovieListId, cancellationToken);
            var nextOrder = maxOrder.Any() ? maxOrder.Max(mli => mli.Order) + 1 : 1;

            var movieListItem = new MovieListItem
            {
                Id = Guid.NewGuid(),
                MovieListId = request.MovieListId,
                MovieId = request.MovieId,
                Notes = request.Notes ?? "",
                Order = nextOrder,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.MovieListItems.AddAsync(movieListItem, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var result = new MovieListItemResult(
                movieListItem.Id,
                movieListItem.MovieListId,
                movieListItem.MovieId,
                movieListItem.Notes,
                movieListItem.Order,
                movieListItem.CreatedAt,
                movie.Title,
                movie.PosterPath ?? ""
            );

            return Result<MovieListItemResult>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<MovieListItemResult>.Failure($"Film listeye eklenirken hata oluştu: {ex.Message}");
        }
    }
}