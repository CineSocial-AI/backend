using CineSocial.Application.Common.Models;
using MediatR;

namespace CineSocial.Application.Features.Users.Queries.GetAllUsers;

public record GetAllUsersQuery(string? SearchTerm, int Page = 1, int PageSize = 20) : IRequest<Result<object>>;
