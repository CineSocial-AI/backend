using CineSocial.Application.Common.Models;
using MediatR;

namespace CineSocial.Application.Features.Reactions.Queries.GetCommentReactions;

public record GetCommentReactionsQuery(int CommentId) : IRequest<Result<object>>;
