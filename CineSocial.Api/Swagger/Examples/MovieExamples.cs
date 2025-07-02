using CineSocial.Api.DTOs;
using Swashbuckle.AspNetCore.Filters;

namespace CineSocial.Api.Swagger.Examples;

public class MovieDtoExample : IExamplesProvider<MovieDto>
{
    public MovieDto GetExamples()
    {
        return new MovieDto
        {
            Id = Guid.Parse("550e8400-e29b-41d4-a716-446655440000"),
            Title = "Inception",
            OriginalTitle = "Inception",
            Overview = "A thief who steals corporate secrets through dream-sharing technology is given the inverse task of planting an idea into the mind of a C.E.O.",
            ReleaseDate = new DateTime(2010, 7, 16),
            Runtime = 148,
            VoteAverage = 8.8m,
            VoteCount = 32000,
            Language = "en",
            Popularity = 95.3m,
            Status = "Released",
            Budget = 160000000,
            Revenue = 836848102,
            Tagline = "Your mind is the scene of the crime.",
            Genres = new List<GenreDto>
            {
                new GenreDto { Id = Guid.NewGuid(), Name = "Sci-Fi", Description = "Science fiction and futuristic themes" },
                new GenreDto { Id = Guid.NewGuid(), Name = "Thriller", Description = "Suspenseful and exciting movies" }
            },
            Cast = new List<MovieCastDto>
            {
                new MovieCastDto { PersonId = Guid.NewGuid(), Name = "Leonardo DiCaprio", Character = "Dom Cobb", Order = 1 },
                new MovieCastDto { PersonId = Guid.NewGuid(), Name = "Marion Cotillard", Character = "Mal", Order = 2 }
            },
            Crew = new List<MovieCrewDto>
            {
                new MovieCrewDto { PersonId = Guid.NewGuid(), Name = "Christopher Nolan", Job = "Director", Department = "Directing" }
            }
        };
    }
}

public class MovieSearchRequestExample : IExamplesProvider<MovieSearchRequest>
{
    public MovieSearchRequest GetExamples()
    {
        return new MovieSearchRequest
        {
            Query = "inception",
            GenreIds = new List<Guid> { Guid.Parse("550e8400-e29b-41d4-a716-446655440000") },
            Year = 2010,
            Page = 1,
            PageSize = 10,
            SortBy = "popularity",
            SortOrder = "desc"
        };
    }
}

public class MovieListDtoExample : IExamplesProvider<MovieListDto>
{
    public MovieListDto GetExamples()
    {
        return new MovieListDto
        {
            Movies = new List<MovieDto>
            {
                new MovieDto
                {
                    Id = Guid.Parse("550e8400-e29b-41d4-a716-446655440000"),
                    Title = "Inception",
                    OriginalTitle = "Inception",
                    Overview = "A mind-bending thriller about dream manipulation.",
                    ReleaseDate = new DateTime(2010, 7, 16),
                    Runtime = 148,
                    VoteAverage = 8.8m,
                    VoteCount = 32000,
                    Language = "en",
                    Popularity = 95.3m,
                    Status = "Released",
                    Budget = 160000000,
                    Revenue = 836848102,
                    Tagline = "Your mind is the scene of the crime.",
                    Genres = new List<GenreDto>
                    {
                        new GenreDto { Id = Guid.NewGuid(), Name = "Sci-Fi", Description = "Science fiction" }
                    }
                },
                new MovieDto
                {
                    Id = Guid.Parse("660e8400-e29b-41d4-a716-446655440001"),
                    Title = "Barbie",
                    OriginalTitle = "Barbie",
                    Overview = "To live in Barbie Land is to be a perfect being in a perfect place.",
                    ReleaseDate = new DateTime(2023, 7, 21),
                    Runtime = 114,
                    VoteAverage = 7.1m,
                    VoteCount = 8500,
                    Language = "en",
                    Popularity = 88.7m,
                    Status = "Released",
                    Budget = 145000000,
                    Revenue = 1446000000,
                    Tagline = "She's everything. He's just Ken.",
                    Genres = new List<GenreDto>
                    {
                        new GenreDto { Id = Guid.NewGuid(), Name = "Comedy", Description = "Humorous films" }
                    }
                }
            },
            TotalCount = 150,
            Page = 1,
            PageSize = 10,
            TotalPages = 15
        };
    }
}

public class GenreDtoExample : IExamplesProvider<GenreDto>
{
    public GenreDto GetExamples()
    {
        return new GenreDto
        {
            Id = Guid.Parse("550e8400-e29b-41d4-a716-446655440000"),
            Name = "Action",
            Description = "Action movies with thrilling sequences and intense scenes"
        };
    }
}