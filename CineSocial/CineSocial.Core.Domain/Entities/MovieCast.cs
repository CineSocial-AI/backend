using CineSocial.Core.Domain.Common;

namespace CineSocial.Core.Domain.Entities;

public class MovieCast : BaseEntity
{
    public Guid MovieId { get; set; }
    public Guid PersonId { get; set; }
    public string Character { get; set; } = string.Empty;
    public int Order { get; set; }

    public virtual Movie Movie { get; set; } = null!;
    public virtual Person Person { get; set; } = null!;
}
