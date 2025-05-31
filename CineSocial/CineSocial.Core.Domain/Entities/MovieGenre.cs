using CineSocial.Core.Domain.Common;

namespace CineSocial.Core.Domain.Entities;

public class MovieGenre : BaseEntity
{
    public Guid MovieId { get; set; }
    public Guid GenreId { get; set; }

    public virtual Movie Movie { get; set; } = null!;
    public virtual Genre Genre { get; set; } = null!;
}
