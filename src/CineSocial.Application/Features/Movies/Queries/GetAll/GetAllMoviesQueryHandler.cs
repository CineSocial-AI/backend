using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Movie;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.Features.Movies.Queries.GetAll;

public class GetAllMoviesQueryHandler : IRequestHandler<GetAllMoviesQuery, Result<PagedResult<MovieDto>>>
{
    private readonly IRepository<MovieEntity> _movieRepository;

    public GetAllMoviesQueryHandler(IRepository<MovieEntity> movieRepository)
    {
        _movieRepository = movieRepository;
    }

    public async Task<Result<PagedResult<MovieDto>>> Handle(GetAllMoviesQuery request, CancellationToken cancellationToken)
    {
        var query = _movieRepository.GetQueryable().AsQueryable();

        // Search filter
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(m =>
                m.Title.ToLower().Contains(searchTerm) ||
                (m.OriginalTitle != null && m.OriginalTitle.ToLower().Contains(searchTerm)));
        }

        // Sorting
        query = request.SortBy?.ToLower() switch
        {
            "title" => request.SortDescending
                ? query.OrderByDescending(m => m.Title)
                : query.OrderBy(m => m.Title),
            "releasedate" => request.SortDescending
                ? query.OrderByDescending(m => m.ReleaseDate)
                : query.OrderBy(m => m.ReleaseDate),
            "voteaverage" => request.SortDescending
                ? query.OrderByDescending(m => m.VoteAverage)
                : query.OrderBy(m => m.VoteAverage),
            _ => request.SortDescending
                ? query.OrderByDescending(m => m.Popularity)
                : query.OrderBy(m => m.Popularity)
        };

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Pagination
        var movies = await query
            .Skip((request.Page - 1) * 10)
            .Take(10)
            .ToListAsync(cancellationToken);

        var movieDtos = movies.Select(m => new MovieDto
            {
                Id = m.Id,
                TmdbId = m.TmdbId,
                Title = m.Title,
                OriginalTitle = m.OriginalTitle,
                Overview = m.Overview,
                ReleaseDate = m.ReleaseDate,
                Runtime = m.Runtime,
                PosterPath = m.PosterPath,
                BackdropPath = m.BackdropPath,
                Popularity = m.Popularity,
                VoteAverage = m.VoteAverage,
                VoteCount = m.VoteCount
            }).ToList();

        var pagedResult = new PagedResult<MovieDto>(movieDtos, totalCount, request.Page, 10);

        return Result<PagedResult<MovieDto>>.Success(pagedResult);
    }
}
