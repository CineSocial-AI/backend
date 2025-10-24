using CineSocial.Application.Common.Exceptions;
using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Features.Blocks.Queries.GetBlockedUsers;

namespace CineSocial.Application.UseCases.Blocks;

public class GetBlockedUsersUseCase
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetBlockedUsersUseCase(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public List<BlockedUserDto> Execute()
    {
        var currentUserId = _currentUserService.UserId ?? throw new UnauthorizedException("User not authenticated");

        var query = from block in _context.Blocks
                    join blockedUser in _context.Users on block.BlockedUserId equals blockedUser.Id
                    where block.BlockerId == currentUserId
                    orderby block.CreatedAt descending
                    select new BlockedUserDto
                    {
                        UserId = blockedUser.Id,
                        Username = blockedUser.Username,
                        Email = blockedUser.Email,
                        Bio = blockedUser.Bio,
                        ProfileImageId = blockedUser.ProfileImageId,
                        BlockedAt = block.CreatedAt
                    };

        return query.ToList();
    }
}
