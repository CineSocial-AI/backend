using CineSocial.Application.Common.Models;
using MediatR;

namespace CineSocial.Application.Features.Comments.Queries.GetUserComments;

public record GetUserCommentsQuery(int UserId) : IRequest<Result<object>>;
