
using System.Collections.Generic;

namespace CineSocial.Application.Features.Users.Queries.GetAll;

public record UserDto(int Id, string Username, string Email, string? Bio);

public record GetAllUsersResponse(IReadOnlyList<UserDto> Users);
