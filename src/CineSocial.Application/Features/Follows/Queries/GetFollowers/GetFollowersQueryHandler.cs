using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using MediatR;

namespace CineSocial.Application.Features.Follows.Queries.GetFollowers;

public class GetFollowersQueryHandler : IRequestHandler<GetFollowersQuery, Result<List<FollowerDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetFollowersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public Task<Result<List<FollowerDto>>> Handle(GetFollowersQuery request, CancellationToken cancellationToken)
    {
        var query = from follow in _context.Follows
                    join follower in _context.Users on follow.FollowerId equals follower.Id
                    where follow.FollowingId == request.UserId
                    orderby follow.CreatedAt descending
                    select new FollowerDto
                    {
                        UserId = follower.Id,
                        Username = follower.Username,
                        Email = follower.Email,
                        Bio = follower.Bio,
                        ProfileImageId = follower.ProfileImageId,
                        FollowedAt = follow.CreatedAt
                    };

        var totalCount = query.Count();

        var followers = query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var pagination = new PaginationMetadata(
            request.PageNumber,
            request.PageSize,
            totalCount
        );

        return Task.FromResult(Result<List<FollowerDto>>.SuccessPaged(followers, pagination, "Followers retrieved successfully"));
    }
}
