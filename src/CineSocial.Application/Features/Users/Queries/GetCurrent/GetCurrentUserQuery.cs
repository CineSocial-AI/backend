using CineSocial.Application.Common.Models;
using MediatR;

namespace CineSocial.Application.Features.Users.Queries.GetCurrent;

public record GetCurrentUserQuery(int UserId) : IRequest<Result<GetCurrentUserResponse>>;
