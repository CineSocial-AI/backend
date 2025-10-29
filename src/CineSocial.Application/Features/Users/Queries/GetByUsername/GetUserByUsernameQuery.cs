using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.User;
using MediatR;

namespace CineSocial.Application.Features.Users.Queries.GetByUsername;

public record GetUserByUsernameQuery(string Username) : IRequest<Result<AppUser>>;
