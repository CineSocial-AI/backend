using CineSocial.Core.Domain.Common;

namespace CineSocial.Core.Domain.Entities;

public class Person : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Biography { get; set; }
    public DateTime? Birthday { get; set; }
    public DateTime? Deathday { get; set; }
    public string? PlaceOfBirth { get; set; }
    public string? ProfilePath { get; set; }
    public int? TmdbId { get; set; }
    public string? ImdbId { get; set; }
    public decimal? Popularity { get; set; }
    public string? Gender { get; set; }
    public string? KnownForDepartment { get; set; }

    public virtual ICollection<MovieCast> MovieCasts { get; set; } = new List<MovieCast>();
    public virtual ICollection<MovieCrew> MovieCrews { get; set; } = new List<MovieCrew>();
}
