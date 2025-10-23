using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Features.Follows.Queries.GetFollowing;

namespace CineSocial.Application.UseCases.Follows;

public class GetFollowingUseCase
{
    private readonly IApplicationDbContext _context;

    public GetFollowingUseCase(IApplicationDbContext context)
    {
        _context = context;
    }

    public List<FollowingDto> Execute(int userId)
    {
        var query = from follow in _context.Follows
                    join following in _context.Users on follow.FollowingId equals following.Id
                    where follow.FollowerId == userId
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

        return query.ToList();
    }
}
