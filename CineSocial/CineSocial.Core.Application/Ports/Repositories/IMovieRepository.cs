using CineSocial.Core.Domain.Entities;

namespace CineSocial.Core.Application.Ports.Repositories;

/// <summary>
/// Repository interface for Movie-specific operations
/// </summary>
public interface IMovieRepository : IRepository<Movie>
{
    Task<IEnumerable<Movie>> GetMoviesByGenreAsync(Guid genreId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Movie>> GetPopularMoviesAsync(int count, CancellationToken cancellationToken = default);
    Task<IEnumerable<Movie>> GetTopRatedMoviesAsync(int count, CancellationToken cancellationToken = default);
    Task<IEnumerable<Movie>> GetRecentMoviesAsync(int count, CancellationToken cancellationToken = default);
    Task<IEnumerable<Movie>> SearchMoviesAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<Movie?> GetMovieWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByTmdbIdAsync(int tmdbId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Movie>> GetMoviesByGenresAsync(IEnumerable<Guid> genreIds, CancellationToken cancellationToken = default);
}