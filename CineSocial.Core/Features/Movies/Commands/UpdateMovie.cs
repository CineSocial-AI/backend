using CineSocial.Core.Features.Movies.DTOs;
using CineSocial.Core.Shared;
using CineSocial.Core.Shared.Interfaces;
using FluentValidation;
using MediatR;

namespace CineSocial.Core.Features.Movies.Commands;

public record UpdateMovieCommand(UpdateMovieDto Movie) : IRequest<Result<MovieDto>>;

public class UpdateMovieValidator : AbstractValidator<UpdateMovieCommand>
{
    public UpdateMovieValidator()
    {
        RuleFor(x => x.Movie.Id)
            .NotEmpty()
            .WithMessage("Movie ID is required");

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

public class UpdateMovieHandler : IRequestHandler<UpdateMovieCommand, Result<MovieDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMovieHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<MovieDto>> Handle(UpdateMovieCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var movie = await _unitOfWork.Movies.GetByIdAsync(request.Movie.Id, cancellationToken);

            if (movie == null)
            {
                return Result.Failure<MovieDto>(ErrorTypes.NotFound.Movie);
            }

            movie.Title = request.Movie.Title;
            movie.OriginalTitle = request.Movie.OriginalTitle;
            movie.Overview = request.Movie.Overview;
            movie.ReleaseDate = request.Movie.ReleaseDate;
            movie.Runtime = request.Movie.Runtime;
            movie.VoteAverage = request.Movie.VoteAverage;
            movie.VoteCount = request.Movie.VoteCount;
            movie.PosterPath = request.Movie.PosterPath;
            movie.BackdropPath = request.Movie.BackdropPath;
            movie.Language = request.Movie.Language;
            movie.Popularity = request.Movie.Popularity;
            movie.IsAdult = request.Movie.IsAdult;
            movie.Homepage = request.Movie.Homepage;
            movie.Status = request.Movie.Status;
            movie.Budget = request.Movie.Budget;
            movie.Revenue = request.Movie.Revenue;
            movie.Tagline = request.Movie.Tagline;
            movie.ImdbId = request.Movie.ImdbId;
            movie.TmdbId = request.Movie.TmdbId;

            _unitOfWork.Movies.Update(movie);
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