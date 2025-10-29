using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Social;
using MediatR;

namespace CineSocial.Application.Features.Lists.Commands.CreateMovieList;

public record CreateMovieListCommand(
    string Name,
    string? Description,
    bool IsPublic,
    int? CoverImageId
) : IRequest<Result<MovieList>>;
