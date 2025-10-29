using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Social;
using MediatR;

namespace CineSocial.Application.Features.Lists.Commands.CreateMovieList;

public class CreateMovieListCommandHandler : IRequestHandler<CreateMovieListCommand, Result<MovieList>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreateMovieListCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<MovieList>> Handle(CreateMovieListCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _currentUserService.UserId;
            if (currentUserId == null)
                return Result<MovieList>.Failure("User not authenticated");

            if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Length > 200)
                return Result<MovieList>.Failure("Name is required and must be less than 200 characters");

            if (request.Description?.Length > 1000)
                return Result<MovieList>.Failure("Description must be less than 1000 characters");

            var movieList = new MovieList
            {
                UserId = currentUserId.Value,
                Name = request.Name.Trim(),
                Description = request.Description?.Trim(),
                IsPublic = request.IsPublic,
                CoverImageId = request.CoverImageId,
                IsWatchlist = false
            };

            _context.Add(movieList);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<MovieList>.Success(movieList);
        }
        catch (Exception ex)
        {
            return Result<MovieList>.Failure($"Failed to create movie list: {ex.Message}");
        }
    }
}
