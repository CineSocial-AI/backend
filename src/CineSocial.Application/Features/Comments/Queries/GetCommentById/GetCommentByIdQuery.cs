using CineSocial.Application.Common.Models;
using CineSocial.Application.Features.Comments.Queries.GetMovieComments;
using MediatR;
using System.ComponentModel;

namespace CineSocial.Application.Features.Comments.Queries.GetCommentById;

public record GetCommentByIdQuery(
    [property: DefaultValue(1)] int CommentId
) : IRequest<Result<CommentDto>>;
