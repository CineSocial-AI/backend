using CineSocial.Core.Shared;
using CineSocial.Core.Shared.Interfaces;
using CineSocial.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Core.Features.MovieLists.Commands;

public record CreateWatchlistCommand(
    Guid UserId
) : IRequest<Result<MovieListResult>>;

public class CreateWatchlistCommandHandler : IRequestHandler<CreateWatchlistCommand, Result<MovieListResult>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateWatchlistCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<MovieListResult>> Handle(CreateWatchlistCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if user exists
            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                return Result<MovieListResult>.Failure("Kullanıcı bulunamadı.");
            }

            // Check if user already has a watchlist
            var existingWatchlist = await _unitOfWork.MovieLists.FirstOrDefaultAsync(
                ml => ml.UserId == request.UserId && ml.IsWatchlist,
                cancellationToken
            );

            if (existingWatchlist != null)
            {
                // Return existing watchlist
                var existingResult = new MovieListResult(
                    existingWatchlist.Id,
                    existingWatchlist.UserId,
                    existingWatchlist.Name,
                    existingWatchlist.Description ?? "",
                    existingWatchlist.IsPublic,
                    existingWatchlist.IsWatchlist,
                    0, // Count will be calculated separately if needed
                    existingWatchlist.CreatedAt,
                    existingWatchlist.UpdatedAt,
                    $"{user.FirstName} {user.LastName}",
                    user.Username
                );

                return Result<MovieListResult>.Success(existingResult);
            }

            var watchlist = new MovieList
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Name = "İzleme Listesi",
                Description = "Varsayılan izleme listesi",
                IsWatchlist = true,
                IsPublic = true,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.MovieLists.AddAsync(watchlist, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var result = new MovieListResult(
                watchlist.Id,
                watchlist.UserId,
                watchlist.Name,
                watchlist.Description,
                watchlist.IsPublic,
                watchlist.IsWatchlist,
                0, // New watchlist has no movies
                watchlist.CreatedAt,
                watchlist.UpdatedAt,
                $"{user.FirstName} {user.LastName}",
                user.Username
            );

            return Result<MovieListResult>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<MovieListResult>.Failure($"İzleme listesi oluşturulurken hata oluştu: {ex.Message}");
        }
    }
}