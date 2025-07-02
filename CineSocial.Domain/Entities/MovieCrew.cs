namespace CineSocial.Domain.Entities;

public class MovieCrew : BaseEntity
{
    public Guid MovieId { get; set; }
    public Guid PersonId { get; set; }
    public string Job { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;

    public virtual Movie Movie { get; set; } = null!;
    public virtual Person Person { get; set; } = null!;
}