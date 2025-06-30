using CineSocial.Core.Application.DTOs.Common;
using CineSocial.Core.Application.DTOs.Movies;

namespace CineSocial.Core.Application.Services;

/// <summary>
/// Application Service - Movie Service Interface
/// This service encapsulates movie-related business logic
/// </summary>
public interface IMovieService
{
    // Movie operations
    Task<Result<PagedResult<MovieSummaryDto>>> GetMoviesAsync(
        int page = 1,
        int pageSize = 20,
        string? search = null,
        List<Guid>? genreIds = null,
        string? sortBy = null,
        CancellationToken cancellationToken = default);
        
    Task<Result<MovieDto>> GetMovieByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<MovieDto>> CreateMovieAsync(CreateMovieDto createDto, CancellationToken cancellationToken = default);
    Task<Result<MovieDto>> UpdateMovieAsync(Guid id, UpdateMovieDto updateDto, CancellationToken cancellationToken = default);
    Task<Result> DeleteMovieAsync(Guid id, CancellationToken cancellationToken = default);
    
    // Special movie queries
    Task<Result<List<MovieSummaryDto>>> GetPopularMoviesAsync(int count = 10, CancellationToken cancellationToken = default);
    Task<Result<List<MovieSummaryDto>>> GetTopRatedMoviesAsync(int count = 10, CancellationToken cancellationToken = default);
    Task<Result<List<MovieSummaryDto>>> GetRecentMoviesAsync(int count = 10, CancellationToken cancellationToken = default);
    
    // Genre operations
    Task<Result<List<GenreDto>>> GetGenresAsync(CancellationToken cancellationToken = default);
    Task<Result<GenreDto>> CreateGenreAsync(CreateGenreDto createDto, CancellationToken cancellationToken = default);
    
    // Person operations
    Task<Result<List<PersonDto>>> SearchPersonsAsync(string search, CancellationToken cancellationToken = default);
    Task<Result<PersonDto>> CreatePersonAsync(CreatePersonDto createDto, CancellationToken cancellationToken = default);
}