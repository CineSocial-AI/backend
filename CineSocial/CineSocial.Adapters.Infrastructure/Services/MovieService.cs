using Microsoft.EntityFrameworkCore;
using AutoMapper;
using CineSocial.Core.Application.Ports;
using CineSocial.Core.Application.DTOs.Common;
using CineSocial.Core.Application.DTOs.Movies;
using CineSocial.Core.Domain.Entities;
using CineSocial.Adapters.Infrastructure.Database;

namespace CineSocial.Adapters.Infrastructure.Services;

public class MovieService : IMovieService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public MovieService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<MovieSummaryDto>>> GetMoviesAsync(int page = 1, int pageSize = 20, string? search = null, List<Guid>? genreIds = null, string? sortBy = null)
    {
        try
        {
            var query = _context.Movies
                .Include(m => m.MovieGenres)
                .ThenInclude(mg => mg.Genre)
                .Include(m => m.Reviews)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(m => m.Title.Contains(search));
            }

            if (genreIds != null && genreIds.Any())
            {
                query = query.Where(m => m.MovieGenres.Any(mg => genreIds.Contains(mg.GenreId)));
            }

            query = sortBy?.ToLower() switch
            {
                "title" => query.OrderBy(m => m.Title),
                "date" => query.OrderByDescending(m => m.ReleaseDate),
                "rating" => query.OrderByDescending(m => m.VoteAverage),
                _ => query.OrderByDescending(m => m.CreatedAt)
            };

            var totalCount = await query.CountAsync();
            var movies = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var movieDtos = _mapper.Map<List<MovieSummaryDto>>(movies);
            var result = new PagedResult<MovieSummaryDto>(movieDtos, totalCount, page, pageSize);
            return Result<PagedResult<MovieSummaryDto>>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<PagedResult<MovieSummaryDto>>.Failure($"Error getting movies: {ex.Message}");
        }
    }

    public async Task<Result<MovieDto>> GetMovieByIdAsync(Guid id)
    {
        try
        {
            var movie = await _context.Movies
                .Include(m => m.MovieGenres)
                .ThenInclude(mg => mg.Genre)
                .Include(m => m.MovieCasts)
                .ThenInclude(mc => mc.Person)
                .Include(m => m.MovieCrews)
                .ThenInclude(mc => mc.Person)
                .Include(m => m.Reviews)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return Result<MovieDto>.Failure("Movie not found");
            }

            var movieDto = _mapper.Map<MovieDto>(movie);
            return Result<MovieDto>.Success(movieDto);
        }
        catch (Exception ex)
        {
            return Result<MovieDto>.Failure($"Error getting movie: {ex.Message}");
        }
    }

    public async Task<Result<MovieDto>> CreateMovieAsync(CreateMovieDto createDto)
    {
        try
        {
            var movie = _mapper.Map<Movie>(createDto);
            movie.Id = Guid.NewGuid();
            movie.CreatedAt = DateTime.UtcNow;

            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            var createdMovie = await GetMovieByIdAsync(movie.Id);
            return createdMovie;
        }
        catch (Exception ex)
        {
            return Result<MovieDto>.Failure($"Error creating movie: {ex.Message}");
        }
    }

    public async Task<Result<MovieDto>> UpdateMovieAsync(Guid id, UpdateMovieDto updateDto)
    {
        try
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return Result<MovieDto>.Failure("Movie not found");
            }

            _mapper.Map(updateDto, movie);
            movie.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var updatedMovie = await GetMovieByIdAsync(movie.Id);
            return updatedMovie;
        }
        catch (Exception ex)
        {
            return Result<MovieDto>.Failure($"Error updating movie: {ex.Message}");
        }
    }

    public async Task<Result> DeleteMovieAsync(Guid id)
    {
        try
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return Result.Failure("Movie not found");
            }

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error deleting movie: {ex.Message}");
        }
    }

    public async Task<Result<List<GenreDto>>> GetGenresAsync()
    {
        try
        {
            var genres = await _context.Genres.ToListAsync();
            var genreDtos = _mapper.Map<List<GenreDto>>(genres);
            return Result<List<GenreDto>>.Success(genreDtos);
        }
        catch (Exception ex)
        {
            return Result<List<GenreDto>>.Failure($"Error getting genres: {ex.Message}");
        }
    }

    public async Task<Result<GenreDto>> CreateGenreAsync(CreateGenreDto createDto)
    {
        try
        {
            var genre = _mapper.Map<Genre>(createDto);
            genre.Id = Guid.NewGuid();
            genre.CreatedAt = DateTime.UtcNow;

            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();

            var genreDto = _mapper.Map<GenreDto>(genre);
            return Result<GenreDto>.Success(genreDto);
        }
        catch (Exception ex)
        {
            return Result<GenreDto>.Failure($"Error creating genre: {ex.Message}");
        }
    }

    public async Task<Result<List<MovieSummaryDto>>> GetPopularMoviesAsync(int count = 10)
    {
        try
        {
            var movies = await _context.Movies
                .Include(m => m.MovieGenres)
                .ThenInclude(mg => mg.Genre)
                .Include(m => m.Reviews)
                .OrderByDescending(m => m.Popularity)
                .Take(count)
                .ToListAsync();

            var movieDtos = _mapper.Map<List<MovieSummaryDto>>(movies);
            return Result<List<MovieSummaryDto>>.Success(movieDtos);
        }
        catch (Exception ex)
        {
            return Result<List<MovieSummaryDto>>.Failure($"Error getting popular movies: {ex.Message}");
        }
    }

    public async Task<Result<List<MovieSummaryDto>>> GetTopRatedMoviesAsync(int count = 10)
    {
        try
        {
            var movies = await _context.Movies
                .Include(m => m.MovieGenres)
                .ThenInclude(mg => mg.Genre)
                .Include(m => m.Reviews)
                .Where(m => m.Reviews.Any())
                .OrderByDescending(m => m.Reviews.Average(r => r.Rating))
                .Take(count)
                .ToListAsync();

            var movieDtos = _mapper.Map<List<MovieSummaryDto>>(movies);
            return Result<List<MovieSummaryDto>>.Success(movieDtos);
        }
        catch (Exception ex)
        {
            return Result<List<MovieSummaryDto>>.Failure($"Error getting top rated movies: {ex.Message}");
        }
    }

    public async Task<Result<List<MovieSummaryDto>>> GetRecentMoviesAsync(int count = 10)
    {
        try
        {
            var movies = await _context.Movies
                .Include(m => m.MovieGenres)
                .ThenInclude(mg => mg.Genre)
                .Include(m => m.Reviews)
                .OrderByDescending(m => m.CreatedAt)
                .Take(count)
                .ToListAsync();

            var movieDtos = _mapper.Map<List<MovieSummaryDto>>(movies);
            return Result<List<MovieSummaryDto>>.Success(movieDtos);
        }
        catch (Exception ex)
        {
            return Result<List<MovieSummaryDto>>.Failure($"Error getting recent movies: {ex.Message}");
        }
    }

    public async Task<Result<List<PersonDto>>> SearchPersonsAsync(string search)
    {
        try
        {
            var persons = await _context.Persons
                .Where(p => p.Name.Contains(search))
                .Take(20)
                .ToListAsync();

            var personDtos = _mapper.Map<List<PersonDto>>(persons);
            return Result<List<PersonDto>>.Success(personDtos);
        }
        catch (Exception ex)
        {
            return Result<List<PersonDto>>.Failure($"Error searching persons: {ex.Message}");
        }
    }

    public async Task<Result<PersonDto>> CreatePersonAsync(CreatePersonDto createDto)
    {
        try
        {
            var person = _mapper.Map<Person>(createDto);
            person.Id = Guid.NewGuid();
            person.CreatedAt = DateTime.UtcNow;

            _context.Persons.Add(person);
            await _context.SaveChangesAsync();

            var personDto = _mapper.Map<PersonDto>(person);
            return Result<PersonDto>.Success(personDto);
        }
        catch (Exception ex)
        {
            return Result<PersonDto>.Failure($"Error creating person: {ex.Message}");
        }
    }
}

