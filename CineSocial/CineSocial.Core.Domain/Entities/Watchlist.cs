using CineSocial.Core.Domain.Common;

namespace CineSocial.Core.Domain.Entities;

public class Watchlist : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid MovieId { get; set; }
    public bool IsWatched { get; set; }
    public DateTime? WatchedDate { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual Movie Movie { get; set; } = null!;
}
