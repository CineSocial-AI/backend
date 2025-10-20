using CineSocial.Application.Common.Models;
using MediatR;
using System.ComponentModel;

namespace CineSocial.Application.Features.Comments.Queries.GetMovieComments;

public record GetMovieCommentsQuery(
    [property: DefaultValue(1)] int MovieId,
    [property: DefaultValue(1)] int PageNumber = 1,
    [property: DefaultValue(10)] int PageSize = 10
) : IRequest<Result<PagedResult<CommentDto>>>;
