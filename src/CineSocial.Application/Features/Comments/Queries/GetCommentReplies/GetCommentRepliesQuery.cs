using CineSocial.Application.Common.Models;
using CineSocial.Application.Features.Comments.Queries.GetMovieComments;
using MediatR;
using System.ComponentModel;

namespace CineSocial.Application.Features.Comments.Queries.GetCommentReplies;

public record GetCommentRepliesQuery(
    [property: DefaultValue(1)] int CommentId,
    [property: DefaultValue(1)] int PageNumber = 1,
    [property: DefaultValue(10)] int PageSize = 10
) : IRequest<Result<PagedResult<CommentDto>>>;
