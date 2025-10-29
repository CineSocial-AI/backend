using CineSocial.Application.Common.Models;
using MediatR;

namespace CineSocial.Application.Features.Lists.Commands.UpdateMovieList;

public record UpdateMovieListCommand(
    int ListId,
    string? Name,
    string? Description,
    bool? IsPublic,
    int? CoverImageId
) : IRequest<Result<bool>>;
