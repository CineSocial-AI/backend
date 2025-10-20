using CineSocial.Domain.Entities.Social;
using CineSocial.Domain.Entities.User;

namespace CineSocial.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    IQueryable<AppUser> Users { get; }
    IQueryable<Image> Images { get; }
    IQueryable<Follow> Follows { get; }
    IQueryable<Block> Blocks { get; }
    IQueryable<Comment> Comments { get; }
    IQueryable<Reaction> Reactions { get; }
    IQueryable<Rate> Rates { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    void Add<T>(T entity) where T : class;
    void Remove<T>(T entity) where T : class;
    void RemoveRange<T>(IEnumerable<T> entities) where T : class;
}
