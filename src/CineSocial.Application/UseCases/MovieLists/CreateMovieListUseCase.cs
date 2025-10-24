using CineSocial.Application.Common.Exceptions;
using CineSocial.Application.Common.Interfaces;
using CineSocial.Domain.Entities.Social;

namespace CineSocial.Application.UseCases.MovieLists;

public class CreateMovieListUseCase
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreateMovieListUseCase(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<MovieList> ExecuteAsync(
        string name,
        string? description,
        bool isPublic,
        int? coverImageId,
        CancellationToken cancellationToken = default)
    {
        var currentUserId = _currentUserService.UserId ?? throw new UnauthorizedException("User not authenticated");

        if (string.IsNullOrWhiteSpace(name) || name.Length > 200)
            throw new ValidationException("name", "Name is required and must be less than 200 characters");

        if (description?.Length > 1000)
            throw new ValidationException("description", "Description must be less than 1000 characters");

        var movieList = new MovieList
        {
            UserId = currentUserId,
            Name = name.Trim(),
            Description = description?.Trim(),
            IsPublic = isPublic,
            CoverImageId = coverImageId,
            IsWatchlist = false
        };

        _context.Add(movieList);
        await _context.SaveChangesAsync(cancellationToken);

        return movieList;
    }
}
