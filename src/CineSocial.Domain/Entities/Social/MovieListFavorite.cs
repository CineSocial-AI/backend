using CineSocial.Domain.Common;
using CineSocial.Domain.Entities.User;

namespace CineSocial.Domain.Entities.Social;

public class MovieListFavorite : BaseEntity
{
    public int UserId { get; set; }
    public int MovieListId { get; set; }
    public DateTime CreatedAt { get; set; }
    public virtual AppUser User { get; set; }
    public virtual MovieList MovieList { get; set; }
}