using CineSocial.Core.Features.Movies.DTOs;
using CineSocial.Core.Shared;
using CineSocial.Core.Shared.Interfaces;
using FluentValidation;
using MediatR;

namespace CineSocial.Core.Features.Movies.Queries;

public record GetMoviesListQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string? SearchTerm = null,
    string? SortBy = "popularity",
    bool Ascending = false
) : IRequest<PagedResult<MovieListDto>>;

public class GetMoviesListValidator : AbstractValidator<GetMoviesListQuery>
{
    public GetMoviesListValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .WithMessage("Page size must be between 1 and 100");

        RuleFor(x => x.SortBy)
            .Must(x => string.IsNullOrEmpty(x) || new[] { "title", "releasedate", "popularity", "voteaverage" }.Contains(x.ToLower()))
            .WithMessage("Invalid sort field. Allowed values: title, releasedate, popularity, voteaverage");
    }
}

public class GetMoviesListHandler : IRequestHandler<GetMoviesListQuery, PagedResult<MovieListDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetMoviesListHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<MovieListDto>> Handle(GetMoviesListQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var (movies, totalCount) = await _unitOfWork.Movies.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                predicate: !string.IsNullOrEmpty(request.SearchTerm) 
                    ? m => m.Title.Contains(request.SearchTerm) || (m.OriginalTitle != null && m.OriginalTitle.Contains(request.SearchTerm))
                    : null,
                orderBy: request.SortBy?.ToLower() switch
                {
                    "title" => m => m.Title,
                    "releasedate" => m => m.ReleaseDate ?? DateTime.MinValue,
                    "voteaverage" => m => m.VoteAverage ?? 0,
                    _ => m => m.Popularity ?? 0
                },
                ascending: request.Ascending,
                cancellationToken: cancellationToken
            );

            var movieDtos = movies.Select(movie => new MovieListDto(
                movie.Id,
                movie.Title,
                movie.OriginalTitle,
                movie.ReleaseDate,
                movie.VoteAverage,
                movie.PosterPath,
                movie.Popularity
            ));

            return PagedResult<MovieListDto>.Success(
                movieDtos,
                request.PageNumber,
                request.PageSize,
                totalCount
            );
        }
        catch (Exception ex)
        {
            return PagedResult<MovieListDto>.Failure(ErrorTypes.System.DatabaseError);
        }
    }
}