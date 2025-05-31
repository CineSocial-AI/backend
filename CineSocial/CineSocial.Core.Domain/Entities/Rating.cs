using CineSocial.Core.Domain.Common;

namespace CineSocial.Core.Domain.Entities;

public class Rating : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid MovieId { get; set; }
    public decimal Value { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual Movie Movie { get; set; } = null!;
}
