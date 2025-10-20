using CineSocial.Domain.Common;
using CineSocial.Domain.Entities.User;

namespace CineSocial.Domain.Entities.Social;

public class Rate : BaseEntity
{
    public int UserId { get; set; }
    public AppUser User { get; set; } = null!;

    public int MovieId { get; set; }

    public decimal Rating { get; set; }
}
