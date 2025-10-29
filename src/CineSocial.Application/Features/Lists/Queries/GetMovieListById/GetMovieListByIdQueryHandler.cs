using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Social;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.Features.Lists.Queries.GetMovieListById;

public class GetMovieListByIdQueryHandler : IRequestHandler<GetMovieListByIdQuery, Result<MovieList>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetMovieListByIdQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<MovieList>> Handle(GetMovieListByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = _currentUserService.UserId;

            var movieList = await _context.MovieLists
                .Include(ml => ml.Items)
                .ThenInclude(mli => mli.Movie)
                .FirstOrDefaultAsync(ml => ml.Id == request.ListId && !ml.IsDeleted, cancellationToken);

            if (movieList == null)
                return Result<MovieList>.Failure("List not found");

            // Private lists can only be viewed by owner
            if (!movieList.IsPublic && movieList.UserId != currentUserId)
                return Result<MovieList>.Failure("This list is private");

            return Result<MovieList>.Success(movieList);
        }
        catch (Exception ex)
        {
            return Result<MovieList>.Failure($"Failed to retrieve movie list: {ex.Message}");
        }
    }
}
