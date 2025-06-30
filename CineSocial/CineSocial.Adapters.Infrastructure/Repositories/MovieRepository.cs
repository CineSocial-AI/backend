using Microsoft.EntityFrameworkCore;
using CineSocial.Core.Application.Ports.Repositories;
using CineSocial.Core.Domain.Entities;
using CineSocial.Adapters.Infrastructure.Database;

namespace CineSocial.Adapters.Infrastructure.Repositories;

/// <summary>
/// Movie repository implementation with specialized queries
/// </summary>
public class MovieRepository : Repository<Movie>, IMovieRepository
{
    public MovieRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Movie>> GetMoviesByGenreAsync(Guid genreId, CancellationToken cancellationToken = default)
    {
        return await _context.Movies
            .Include(m => m.MovieGenres)
            .ThenInclude(mg => mg.Genre)
            .Include(m => m.Reviews)
            .Where(m => m.MovieGenres.Any(mg => mg.GenreId == genreId))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Movie>> GetPopularMoviesAsync(int count, CancellationToken cancellationToken = default)
    {
        return await _context.Movies
            .Include(m => m.MovieGenres)
            .ThenInclude(mg => mg.Genre)
            .Include(m => m.Reviews)
            .OrderByDescending(m => m.Popularity)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Movie>> GetTopRatedMoviesAsync(int count, CancellationToken cancellationToken = default)
    {
        return await _context.Movies
            .Include(m => m.MovieGenres)
            .ThenInclude(mg => mg.Genre)
            .Include(m => m.Reviews)
            .Where(m => m.Reviews.Any())
            .OrderByDescending(m => m.Reviews.Average(r => r.Rating))
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Movie>> GetRecentMoviesAsync(int count, CancellationToken cancellationToken = default)
    {
        return await _context.Movies
            .Include(m => m.MovieGenres)
            .ThenInclude(mg => mg.Genre)
            .Include(m => m.Reviews)
            .OrderByDescending(m => m.CreatedAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Movie>> SearchMoviesAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _context.Movies
            .Include(m => m.MovieGenres)
            .ThenInclude(mg => mg.Genre)
            .Include(m => m.Reviews)
            .Where(m => m.Title.Contains(searchTerm) || 
                       (m.OriginalTitle != null && m.OriginalTitle.Contains(searchTerm)))
            .ToListAsync(cancellationToken);
    }

    public async Task<Movie?> GetMovieWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Movies
            .Include(m => m.MovieGenres)
            .ThenInclude(mg => mg.Genre)
            .Include(m => m.MovieCasts)
            .ThenInclude(mc => mc.Person)
            .Include(m => m.MovieCrews)
            .ThenInclude(mc => mc.Person)
            .Include(m => m.Reviews)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByTmdbIdAsync(int tmdbId, CancellationToken cancellationToken = default)
    {
        return await _context.Movies
            .AnyAsync(m => m.TmdbId == tmdbId, cancellationToken);
    }

    public async Task<IEnumerable<Movie>> GetMoviesByGenresAsync(IEnumerable<Guid> genreIds, CancellationToken cancellationToken = default)
    {
        return await _context.Movies
            .Include(m => m.MovieGenres)
            .ThenInclude(mg => mg.Genre)
            .Include(m => m.Reviews)
            .Where(m => m.MovieGenres.Any(mg => genreIds.Contains(mg.GenreId)))
            .ToListAsync(cancellationToken);
    }
}