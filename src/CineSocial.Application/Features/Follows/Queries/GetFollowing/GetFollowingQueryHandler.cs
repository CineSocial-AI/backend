using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using MediatR;

namespace CineSocial.Application.Features.Follows.Queries.GetFollowing;

public class GetFollowingQueryHandler : IRequestHandler<GetFollowingQuery, Result<List<FollowingDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetFollowingQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public Task<Result<List<FollowingDto>>> Handle(GetFollowingQuery request, CancellationToken cancellationToken)
    {
        var query = from follow in _context.Follows
                    join following in _context.Users on follow.FollowingId equals following.Id
                    where follow.FollowerId == request.UserId
                    orderby follow.CreatedAt descending
                    select new FollowingDto
                    {
                        UserId = following.Id,
                        Username = following.Username,
                        Email = following.Email,
                        Bio = following.Bio,
                        ProfileImageId = following.ProfileImageId,
                        FollowedAt = follow.CreatedAt
                    };

        var totalCount = query.Count();

        var followingList = query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var pagination = new PaginationMetadata(
            request.PageNumber,
            request.PageSize,
            totalCount
        );

        return Task.FromResult(Result<List<FollowingDto>>.SuccessPaged(followingList, pagination, "Following retrieved successfully"));
    }
}
