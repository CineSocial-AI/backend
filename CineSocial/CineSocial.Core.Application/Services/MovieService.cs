using AutoMapper;
using Microsoft.Extensions.Logging;
using CineSocial.Core.Application.DTOs.Common;
using CineSocial.Core.Application.DTOs.Movies;
using CineSocial.Core.Application.Ports.Repositories;
using CineSocial.Core.Domain.Entities;

namespace CineSocial.Core.Application.Services;

/// <summary>
/// Movie service implementation containing business logic
/// </summary>
public class MovieService : IMovieService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<MovieService> _logger;

    public MovieService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<MovieService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<PagedResult<MovieSummaryDto>>> GetMoviesAsync(
        int page = 1,
        int pageSize = 20,
        string? search = null,
        List<Guid>? genreIds = null,
        string? sortBy = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate input parameters
            if (page <= 0) page = 1;
            if (pageSize <= 0 || pageSize > 100) pageSize = 20;

            IEnumerable<Movie> movies;
            int totalCount;

            // Apply search and genre filters
            if (!string.IsNullOrEmpty(search))
            {
                movies = await _unitOfWork.Movies.SearchMoviesAsync(search, cancellationToken);
            }
            else if (genreIds != null && genreIds.Any())
            {
                movies = await _unitOfWork.Movies.GetMoviesByGenresAsync(genreIds, cancellationToken);
            }
            else
            {
                movies = await _unitOfWork.Movies.GetAllAsync(cancellationToken);
            }

            // Apply sorting
            movies = sortBy?.ToLower() switch
            {
                "title" => movies.OrderBy(m => m.Title),
                "date" => movies.OrderByDescending(m => m.ReleaseDate),
                "rating" => movies.OrderByDescending(m => m.GetAverageRating()),
                _ => movies.OrderByDescending(m => m.CreatedAt)
            };

            totalCount = movies.Count();

            // Apply pagination
            var pagedMovies = movies
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var movieDtos = _mapper.Map<List<MovieSummaryDto>>(pagedMovies);
            var result = new PagedResult<MovieSummaryDto>(movieDtos, totalCount, page, pageSize);

            return Result<PagedResult<MovieSummaryDto>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting movies");
            return Result<PagedResult<MovieSummaryDto>>.Failure("Filmler alınırken hata oluştu");
        }
    }

    public async Task<Result<MovieDto>> GetMovieByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var movie = await _unitOfWork.Movies.GetMovieWithDetailsAsync(id, cancellationToken);
            if (movie == null)
            {
                return Result<MovieDto>.Failure("Film bulunamadı");
            }

            var movieDto = _mapper.Map<MovieDto>(movie);
            return Result<MovieDto>.Success(movieDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting movie by id: {MovieId}", id);
            return Result<MovieDto>.Failure("Film alınırken hata oluştu");
        }
    }

    public async Task<Result<MovieDto>> CreateMovieAsync(CreateMovieDto createDto, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate business rules
            if (createDto.TmdbId.HasValue && await _unitOfWork.Movies.ExistsByTmdbIdAsync(createDto.TmdbId.Value, cancellationToken))
            {
                return Result<MovieDto>.Failure("Bu TMDB ID'sine sahip film zaten mevcut");
            }

            var movie = _mapper.Map<Movie>(createDto);
            await _unitOfWork.Movies.AddAsync(movie, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var movieDto = await GetMovieByIdAsync(movie.Id, cancellationToken);
            return movieDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating movie");
            return Result<MovieDto>.Failure("Film oluşturulurken hata oluştu");
        }
    }

    public async Task<Result<MovieDto>> UpdateMovieAsync(Guid id, UpdateMovieDto updateDto, CancellationToken cancellationToken = default)
    {
        try
        {
            var movie = await _unitOfWork.Movies.GetByIdAsync(id, cancellationToken);
            if (movie == null)
            {
                return Result<MovieDto>.Failure("Film bulunamadı");
            }

            _mapper.Map(updateDto, movie);
            movie.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Movies.Update(movie);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var movieDto = await GetMovieByIdAsync(movie.Id, cancellationToken);
            return movieDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating movie: {MovieId}", id);
            return Result<MovieDto>.Failure("Film güncellenirken hata oluştu");
        }
    }

    public async Task<Result> DeleteMovieAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var movie = await _unitOfWork.Movies.GetByIdAsync(id, cancellationToken);
            if (movie == null)
            {
                return Result.Failure("Film bulunamadı");
            }

            _unitOfWork.Movies.Remove(movie);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting movie: {MovieId}", id);
            return Result.Failure("Film silinirken hata oluştu");
        }
    }

    public async Task<Result<List<MovieSummaryDto>>> GetPopularMoviesAsync(int count = 10, CancellationToken cancellationToken = default)
    {
        try
        {
            var movies = await _unitOfWork.Movies.GetPopularMoviesAsync(count, cancellationToken);
            var movieDtos = _mapper.Map<List<MovieSummaryDto>>(movies);
            return Result<List<MovieSummaryDto>>.Success(movieDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting popular movies");
            return Result<List<MovieSummaryDto>>.Failure("Popüler filmler alınırken hata oluştu");
        }
    }

    public async Task<Result<List<MovieSummaryDto>>> GetTopRatedMoviesAsync(int count = 10, CancellationToken cancellationToken = default)
    {
        try
        {
            var movies = await _unitOfWork.Movies.GetTopRatedMoviesAsync(count, cancellationToken);
            var movieDtos = _mapper.Map<List<MovieSummaryDto>>(movies);
            return Result<List<MovieSummaryDto>>.Success(movieDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting top rated movies");
            return Result<List<MovieSummaryDto>>.Failure("En iyi filmler alınırken hata oluştu");
        }
    }

    public async Task<Result<List<MovieSummaryDto>>> GetRecentMoviesAsync(int count = 10, CancellationToken cancellationToken = default)
    {
        try
        {
            var movies = await _unitOfWork.Movies.GetRecentMoviesAsync(count, cancellationToken);
            var movieDtos = _mapper.Map<List<MovieSummaryDto>>(movies);
            return Result<List<MovieSummaryDto>>.Success(movieDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recent movies");
            return Result<List<MovieSummaryDto>>.Failure("Son filmler alınırken hata oluştu");
        }
    }

    public async Task<Result<List<GenreDto>>> GetGenresAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var genres = await _unitOfWork.Genres.GetAllAsync(cancellationToken);
            var genreDtos = _mapper.Map<List<GenreDto>>(genres);
            return Result<List<GenreDto>>.Success(genreDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting genres");
            return Result<List<GenreDto>>.Failure("Türler alınırken hata oluştu");
        }
    }

    public async Task<Result<GenreDto>> CreateGenreAsync(CreateGenreDto createDto, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if genre with same name exists
            var existingGenre = await _unitOfWork.Genres.FirstOrDefaultAsync(g => g.Name == createDto.Name, cancellationToken);
            if (existingGenre != null)
            {
                return Result<GenreDto>.Failure("Bu isimde bir tür zaten mevcut");
            }

            var genre = _mapper.Map<Genre>(createDto);
            await _unitOfWork.Genres.AddAsync(genre, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var genreDto = _mapper.Map<GenreDto>(genre);
            return Result<GenreDto>.Success(genreDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating genre");
            return Result<GenreDto>.Failure("Tür oluşturulurken hata oluştu");
        }
    }

    public async Task<Result<List<PersonDto>>> SearchPersonsAsync(string search, CancellationToken cancellationToken = default)
    {
        try
        {
            // TODO: Implement person repository and search
            await Task.CompletedTask;
            return Result<List<PersonDto>>.Success(new List<PersonDto>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching persons");
            return Result<List<PersonDto>>.Failure("Kişiler aranırken hata oluştu");
        }
    }

    public async Task<Result<PersonDto>> CreatePersonAsync(CreatePersonDto createDto, CancellationToken cancellationToken = default)
    {
        try
        {
            // TODO: Implement person creation
            await Task.CompletedTask;
            return Result<PersonDto>.Failure("Henüz implement edilmedi");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating person");
            return Result<PersonDto>.Failure("Kişi oluşturulurken hata oluştu");
        }
    }
}