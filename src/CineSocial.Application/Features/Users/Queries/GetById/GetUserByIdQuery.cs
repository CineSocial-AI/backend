using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.User;
using MediatR;

namespace CineSocial.Application.Features.Users.Queries.GetById;

public record GetUserByIdQuery(int Id) : IRequest<Result<AppUser>>;
