using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.User;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.Features.Users.Queries.GetByUsername;

public class GetUserByUsernameQueryHandler : IRequestHandler<GetUserByUsernameQuery, Result<AppUser>>
{
    private readonly IRepository<AppUser> _userRepository;

    public GetUserByUsernameQueryHandler(IRepository<AppUser> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<AppUser>> Handle(GetUserByUsernameQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetQueryable()
                .Include(u => u.Followers)
                .Include(u => u.Following)
                .FirstOrDefaultAsync(u => u.Username == request.Username && !u.IsDeleted, cancellationToken);

            if (user == null)
                return Result<AppUser>.Failure("User not found");

            return Result<AppUser>.Success(user);
        }
        catch (Exception ex)
        {
            return Result<AppUser>.Failure($"Failed to retrieve user: {ex.Message}");
        }
    }
}
