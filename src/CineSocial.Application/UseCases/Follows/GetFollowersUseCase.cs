using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Features.Follows.Queries.GetFollowers;

namespace CineSocial.Application.UseCases.Follows;

public class GetFollowersUseCase
{
    private readonly IApplicationDbContext _context;

    public GetFollowersUseCase(IApplicationDbContext context)
    {
        _context = context;
    }

    public List<FollowerDto> Execute(int userId)
    {
        var query = from follow in _context.Follows
                    join follower in _context.Users on follow.FollowerId equals follower.Id
                    where follow.FollowingId == userId
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

        return query.ToList();
    }
}
