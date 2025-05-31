using CineSocial.Core.Domain.Common;

namespace CineSocial.Core.Domain.Entities;

public class PostTag : BaseEntity
{
    public Guid PostId { get; set; }
    public string Tag { get; set; } = string.Empty;

    public virtual Post Post { get; set; } = null!;
}
