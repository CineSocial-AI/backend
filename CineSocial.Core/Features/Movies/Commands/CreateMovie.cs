using CineSocial.Core.Features.Movies.DTOs;
using CineSocial.Core.Shared;
using CineSocial.Core.Shared.Interfaces;
using CineSocial.Domain.Entities;
using FluentValidation;
using MediatR;

namespace CineSocial.Core.Features.Movies.Commands;

public record CreateMovieCommand(CreateMovieDto Movie) : IRequest<Result<MovieDto>>;

public class CreateMovieValidator : AbstractValidator<CreateMovieCommand>
{
    public CreateMovieValidator()
    {
        RuleFor(x => x.Movie.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .MaximumLength(500)
            .WithMessage("Title cannot exceed 500 characters");

        RuleFor(x => x.Movie.OriginalTitle)
            .MaximumLength(500)
            .WithMessage("Original title cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Movie.OriginalTitle));

        RuleFor(x => x.Movie.Overview)
            .MaximumLength(2000)
            .WithMessage("Overview cannot exceed 2000 characters")
            .When(x => !string.IsNullOrEmpty(x.Movie.Overview));

        RuleFor(x => x.Movie.Runtime)
            .GreaterThan(0)
            .WithMessage("Runtime must be greater than 0")
            .When(x => x.Movie.Runtime.HasValue);

        RuleFor(x => x.Movie.VoteAverage)
            .InclusiveBetween(0, 10)
            .WithMessage("Vote average must be between 0 and 10")
            .When(x => x.Movie.VoteAverage.HasValue);

        RuleFor(x => x.Movie.VoteCount)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Vote count cannot be negative")
            .When(x => x.Movie.VoteCount.HasValue);

        RuleFor(x => x.Movie.Budget)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Budget cannot be negative")
            .When(x => x.Movie.Budget.HasValue);

        RuleFor(x => x.Movie.Revenue)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Revenue cannot be negative")
            .When(x => x.Movie.Revenue.HasValue);
    }
}

public class CreateMovieHandler : IRequestHandler<CreateMovieCommand, Result<MovieDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateMovieHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<MovieDto>> Handle(CreateMovieCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingMovie = await _unitOfWork.Movies
                .FirstOrDefaultAsync(m => m.TmdbId == request.Movie.TmdbId && request.Movie.TmdbId.HasValue, cancellationToken);

            if (existingMovie != null)
            {
                return Result.Failure<MovieDto>(ErrorTypes.Conflict.UserAlreadyExists);
            }

            var movie = new Movie
            {
                Title = request.Movie.Title,
                OriginalTitle = request.Movie.OriginalTitle,
                Overview = request.Movie.Overview,
                ReleaseDate = request.Movie.ReleaseDate,
                Runtime = request.Movie.Runtime,
                VoteAverage = request.Movie.VoteAverage,
                VoteCount = request.Movie.VoteCount,
                PosterPath = request.Movie.PosterPath,
                BackdropPath = request.Movie.BackdropPath,
                Language = request.Movie.Language,
                Popularity = request.Movie.Popularity,
                IsAdult = request.Movie.IsAdult,
                Homepage = request.Movie.Homepage,
                Status = request.Movie.Status,
                Budget = request.Movie.Budget,
                Revenue = request.Movie.Revenue,
                Tagline = request.Movie.Tagline,
                ImdbId = request.Movie.ImdbId,
                TmdbId = request.Movie.TmdbId
            };

            await _unitOfWork.Movies.AddAsync(movie, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var movieDto = new MovieDto(
                movie.Id,
                movie.Title,
                movie.OriginalTitle,
                movie.Overview,
                movie.ReleaseDate,
                movie.Runtime,
                movie.VoteAverage,
                movie.VoteCount,
                movie.PosterPath,
                movie.BackdropPath,
                movie.Language,
                movie.Popularity,
                movie.IsAdult,
                movie.Homepage,
                movie.Status,
                movie.Budget,
                movie.Revenue,
                movie.Tagline,
                movie.ImdbId,
                movie.TmdbId,
                movie.CreatedAt,
                movie.UpdatedAt
            );

            return Result.Success(movieDto);
        }
        catch (Exception ex)
        {
            return Result.Failure<MovieDto>(ErrorTypes.System.DatabaseError);
        }
    }
}