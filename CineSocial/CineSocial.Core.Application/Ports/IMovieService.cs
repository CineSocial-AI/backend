using CineSocial.Core.Application.DTOs.Common;
using CineSocial.Core.Application.DTOs.Movies;

namespace CineSocial.Core.Application.Ports;

public interface IMovieService
{
    Task<Result<PagedResult<MovieSummaryDto>>> GetMoviesAsync(int page = 1, int pageSize = 20, string? search = null, List<Guid>? genreIds = null, string? sortBy = null);
    Task<Result<MovieDto>> GetMovieByIdAsync(Guid id);
    Task<Result<MovieDto>> CreateMovieAsync(CreateMovieDto createDto);
    Task<Result<MovieDto>> UpdateMovieAsync(Guid id, UpdateMovieDto updateDto);
    Task<Result> DeleteMovieAsync(Guid id);
    Task<Result<List<GenreDto>>> GetGenresAsync();
    Task<Result<GenreDto>> CreateGenreAsync(CreateGenreDto createDto);
    Task<Result<List<MovieSummaryDto>>> GetPopularMoviesAsync(int count = 10);
    Task<Result<List<MovieSummaryDto>>> GetTopRatedMoviesAsync(int count = 10);
    Task<Result<List<MovieSummaryDto>>> GetRecentMoviesAsync(int count = 10);
    Task<Result<List<PersonDto>>> SearchPersonsAsync(string search);
    Task<Result<PersonDto>> CreatePersonAsync(CreatePersonDto createDto);
}

