namespace CineSocial.Domain.Entities.Movie;

public class MovieProductionCompany
{
    public int MovieId { get; set; }
    public MovieEntity Movie { get; set; } = null!;

    public int ProductionCompanyId { get; set; }
    public ProductionCompany ProductionCompany { get; set; } = null!;
}
