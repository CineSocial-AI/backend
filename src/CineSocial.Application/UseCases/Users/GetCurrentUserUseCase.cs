using CineSocial.Application.Common.Exceptions;
using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Features.Users.Queries.GetCurrent;
using CineSocial.Domain.Entities.User;

namespace CineSocial.Application.UseCases.Users;

public class GetCurrentUserUseCase
{
    private readonly IRepository<AppUser> _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetCurrentUserUseCase(IRepository<AppUser> userRepository, ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<GetCurrentUserResponse> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.UserId ?? throw new UnauthorizedException("User not authenticated");

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

        if (user == null)
            throw new NotFoundException("User", userId);

        return new GetCurrentUserResponse(
            user.Id,
            user.Username,
            user.Email,
            user.Role.ToString(),
            user.Bio,
            user.ProfileImageId,
            user.BackgroundImageId,
            user.LastLoginAt,
            user.CreatedAt
        );
    }
}
