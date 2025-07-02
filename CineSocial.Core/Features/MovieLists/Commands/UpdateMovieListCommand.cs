using CineSocial.Core.Shared;
using CineSocial.Core.Shared.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Core.Features.MovieLists.Commands;

public record UpdateMovieListCommand(
    Guid Id,
    Guid UserId,
    string Name,
    string? Description = null,
    bool IsPublic = true
) : IRequest<Result<MovieListResult>>;

public class UpdateMovieListCommandHandler : IRequestHandler<UpdateMovieListCommand, Result<MovieListResult>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMovieListCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<MovieListResult>> Handle(UpdateMovieListCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var movieList = await _unitOfWork.MovieLists.GetByIdAsync(request.Id, cancellationToken);
            if (movieList == null)
            {
                return Result<MovieListResult>.Failure("Liste bulunamadı.");
            }

            // Check if user is the owner of the list
            if (movieList.UserId != request.UserId)
            {
                return Result<MovieListResult>.Failure("Bu listeyi güncelleme yetkiniz yok.");
            }

            var user = await _unitOfWork.Users.GetByIdAsync(movieList.UserId, cancellationToken);
            if (user == null)
            {
                return Result<MovieListResult>.Failure("Kullanıcı bulunamadı.");
            }

            // Check if another list with the same name exists (excluding current list)
            var existingList = await _unitOfWork.MovieLists.FirstOrDefaultAsync(
                ml => ml.UserId == request.UserId && 
                      ml.Name.ToLower() == request.Name.ToLower() && 
                      ml.Id != request.Id,
                cancellationToken
            );

            if (existingList != null)
            {
                return Result<MovieListResult>.Failure("Bu isimde başka bir listeniz zaten mevcut.");
            }

            // Get movie count
            var movieCount = await _unitOfWork.MovieListItems.CountAsync(
                mli => mli.MovieListId == movieList.Id,
                cancellationToken
            );

            movieList.Name = request.Name;
            movieList.Description = request.Description ?? "";
            movieList.IsPublic = request.IsPublic;
            movieList.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.MovieLists.Update(movieList);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var result = new MovieListResult(
                movieList.Id,
                movieList.UserId,
                movieList.Name,
                movieList.Description,
                movieList.IsPublic,
                movieList.IsWatchlist,
                movieCount,
                movieList.CreatedAt,
                movieList.UpdatedAt,
                $"{user.FirstName} {user.LastName}",
                user.Username
            );

            return Result<MovieListResult>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<MovieListResult>.Failure($"Liste güncellenirken hata oluştu: {ex.Message}");
        }
    }
}