using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.User;
using MediatR;

namespace CineSocial.Application.Features.Users.Queries.GetCurrent;

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, Result<GetCurrentUserResponse>>
{
    private readonly IRepository<AppUser> _userRepository;

    public GetCurrentUserQueryHandler(IRepository<AppUser> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<GetCurrentUserResponse>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user == null)
            return Result<GetCurrentUserResponse>.Failure("User not found");

        var response = new GetCurrentUserResponse(
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

        return Result<GetCurrentUserResponse>.Success(response);
    }
}
