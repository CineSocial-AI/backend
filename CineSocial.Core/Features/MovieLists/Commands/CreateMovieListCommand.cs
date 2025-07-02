using CineSocial.Core.Shared;
using CineSocial.Core.Shared.Interfaces;
using CineSocial.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Core.Features.MovieLists.Commands;

public record CreateMovieListCommand(
    Guid UserId,
    string Name,
    string? Description = null,
    bool IsPublic = true
) : IRequest<Result<MovieListResult>>;

public record MovieListResult(
    Guid Id,
    Guid UserId,
    string Name,
    string Description,
    bool IsPublic,
    bool IsWatchlist,
    int MovieCount,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string UserFullName,
    string UserUsername
);

public class CreateMovieListCommandHandler : IRequestHandler<CreateMovieListCommand, Result<MovieListResult>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateMovieListCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<MovieListResult>> Handle(CreateMovieListCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if user exists
            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                return Result<MovieListResult>.Failure("Kullanıcı bulunamadı.");
            }

            // Check if user already has a list with the same name
            var existingList = await _unitOfWork.MovieLists.FirstOrDefaultAsync(
                ml => ml.UserId == request.UserId && ml.Name.ToLower() == request.Name.ToLower(),
                cancellationToken
            );

            if (existingList != null)
            {
                return Result<MovieListResult>.Failure("Bu isimde bir listeniz zaten mevcut.");
            }

            var movieList = new MovieList
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Name = request.Name,
                Description = request.Description ?? "",
                IsPublic = request.IsPublic,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.MovieLists.AddAsync(movieList, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var result = new MovieListResult(
                movieList.Id,
                movieList.UserId,
                movieList.Name,
                movieList.Description,
                movieList.IsPublic,
                movieList.IsWatchlist,
                0, // New list has no movies
                movieList.CreatedAt,
                movieList.UpdatedAt,
                $"{user.FirstName} {user.LastName}",
                user.Username
            );

            return Result<MovieListResult>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<MovieListResult>.Failure($"Liste oluşturulurken hata oluştu: {ex.Message}");
        }
    }
}