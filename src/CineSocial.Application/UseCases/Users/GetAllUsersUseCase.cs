using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Features.Users.Queries.GetAll;
using CineSocial.Domain.Entities.User;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CineSocial.Application.UseCases.Users;

public class GetAllUsersUseCase
{
    private readonly IRepository<AppUser> _userRepository;

    public GetAllUsersUseCase(IRepository<AppUser> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IReadOnlyList<UserDto>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);

        var userDtos = users.Select(user => new UserDto(
            user.Id,
            user.Username,
            user.Email,
            user.Bio
        )).ToList();

        return userDtos;
    }
}
