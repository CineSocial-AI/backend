using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using MediatR;

namespace CineSocial.Application.Features.Blocks.Queries.GetBlockedUsers;

public class GetBlockedUsersQueryHandler : IRequestHandler<GetBlockedUsersQuery, Result<List<BlockedUserDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetBlockedUsersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public Task<Result<List<BlockedUserDto>>> Handle(GetBlockedUsersQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = 1; // TODO: Get from HttpContext

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

        var totalCount = query.Count();

        var blockedUsers = query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var pagination = new PaginationMetadata(
            request.PageNumber,
            request.PageSize,
            totalCount
        );

        return Task.FromResult(Result<List<BlockedUserDto>>.SuccessPaged(blockedUsers, pagination, "Blocked users retrieved successfully"));
    }
}
