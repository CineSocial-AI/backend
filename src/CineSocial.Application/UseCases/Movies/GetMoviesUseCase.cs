using CineSocial.Application.Common.Interfaces;
using CineSocial.Domain.Entities.Movie;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.UseCases.Movies;

public class GetMoviesUseCase
{
    private readonly IRepository<MovieEntity> _movieRepository;

    public GetMoviesUseCase(IRepository<MovieEntity> movieRepository)
    {
        _movieRepository = movieRepository;
    }

    public IQueryable<MovieEntity> Execute(
        string? searchTerm = null,
        string? sortBy = "Popularity",
        bool sortDescending = true)
    {
        var query = _movieRepository.GetQueryable()
            .Include(m => m.MovieGenres)
                .ThenInclude(mg => mg.Genre)
            .AsQueryable();

        // Search filter
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(m =>
                m.Title.ToLower().Contains(term) ||
                (m.OriginalTitle != null && m.OriginalTitle.ToLower().Contains(term)));
        }

        // Sorting
        query = sortBy?.ToLower() switch
        {
            "title" => sortDescending
                ? query.OrderByDescending(m => m.Title)
                : query.OrderBy(m => m.Title),
            "releasedate" => sortDescending
                ? query.OrderByDescending(m => m.ReleaseDate)
                : query.OrderBy(m => m.ReleaseDate),
            "voteaverage" => sortDescending
                ? query.OrderByDescending(m => m.VoteAverage)
                : query.OrderBy(m => m.VoteAverage),
            _ => sortDescending
                ? query.OrderByDescending(m => m.Popularity)
                : query.OrderBy(m => m.Popularity)
        };

        return query; // IQueryable döner, GraphQL projection uygular
    }
}
