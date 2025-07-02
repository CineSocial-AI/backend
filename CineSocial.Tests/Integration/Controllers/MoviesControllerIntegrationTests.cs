using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CineSocial.Api.DTOs;
using CineSocial.Domain.Entities;
using CineSocial.Tests.Integration.Infrastructure;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CineSocial.Tests.Integration.Controllers;

public class MoviesControllerIntegrationTests : IntegrationTestBase
{
    public MoviesControllerIntegrationTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetMovies_WithoutParameters_ShouldReturnMoviesList()
    {
        // Arrange
        await ClearDatabaseAsync();
        await CreateTestMoviesWithGenresAsync();

        // Act
        var response = await Client.GetAsync("/api/movies");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var movieList = JsonSerializer.Deserialize<MovieListDto>(content, JsonOptions);

        movieList.Should().NotBeNull();
        movieList!.Movies.Should().NotBeEmpty();
        movieList.TotalCount.Should().BeGreaterThan(0);
        movieList.Page.Should().Be(1);
        movieList.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task GetMovies_WithPagination_ShouldReturnCorrectPage()
    {
        // Arrange
        await ClearDatabaseAsync();
        await CreateTestMoviesWithGenresAsync(15); // Create 15 movies

        // Act
        var response = await Client.GetAsync("/api/movies?page=2&pageSize=5");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var movieList = JsonSerializer.Deserialize<MovieListDto>(content, JsonOptions);

        movieList.Should().NotBeNull();
        movieList!.Page.Should().Be(2);
        movieList.PageSize.Should().Be(5);
        movieList.Movies.Should().HaveCount(5);
        movieList.TotalCount.Should().Be(15);
        movieList.TotalPages.Should().Be(3);
    }

    [Fact]
    public async Task GetMovies_WithQueryFilter_ShouldReturnFilteredResults()
    {
        // Arrange
        await ClearDatabaseAsync();
        await CreateTestMovieAsync("Action Movie");
        await CreateTestMovieAsync("Comedy Film");
        await CreateTestMovieAsync("Action Hero");

        // Act
        var response = await Client.GetAsync("/api/movies?query=Action");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var movieList = JsonSerializer.Deserialize<MovieListDto>(content, JsonOptions);

        movieList.Should().NotBeNull();
        movieList!.Movies.Should().HaveCount(2);
        movieList.Movies.Should().OnlyContain(m => m.Title.Contains("Action"));
    }

    [Fact]
    public async Task GetMovies_WithGenreFilter_ShouldReturnFilteredResults()
    {
        // Arrange
        await ClearDatabaseAsync();
        var actionGenre = await CreateTestGenreAsync("Action");
        var comedyGenre = await CreateTestGenreAsync("Comedy");

        var actionMovie = await CreateTestMovieAsync("Action Movie");
        var comedyMovie = await CreateTestMovieAsync("Comedy Movie");

        // Add genres to movies
        DbContext.MovieGenres.Add(new MovieGenre { MovieId = actionMovie.Id, GenreId = actionGenre.Id });
        DbContext.MovieGenres.Add(new MovieGenre { MovieId = comedyMovie.Id, GenreId = comedyGenre.Id });
        await DbContext.SaveChangesAsync();

        // Act
        var response = await Client.GetAsync($"/api/movies?genreIds={actionGenre.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var movieList = JsonSerializer.Deserialize<MovieListDto>(content, JsonOptions);

        movieList.Should().NotBeNull();
        movieList!.Movies.Should().HaveCount(1);
        movieList.Movies.First().Title.Should().Be("Action Movie");
    }

    [Fact]
    public async Task GetMovies_WithYearFilter_ShouldReturnFilteredResults()
    {
        // Arrange
        await ClearDatabaseAsync();
        var movie2023 = await CreateTestMovieAsync("Movie 2023");
        movie2023.ReleaseDate = new DateTime(2023, 1, 1);

        var movie2024 = await CreateTestMovieAsync("Movie 2024");
        movie2024.ReleaseDate = new DateTime(2024, 1, 1);

        await DbContext.SaveChangesAsync();

        // Act
        var response = await Client.GetAsync("/api/movies?year=2023");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var movieList = JsonSerializer.Deserialize<MovieListDto>(content, JsonOptions);

        movieList.Should().NotBeNull();
        movieList!.Movies.Should().HaveCount(1);
        movieList.Movies.First().Title.Should().Be("Movie 2023");
    }

    [Fact]
    public async Task GetMovies_WithSorting_ShouldReturnSortedResults()
    {
        // Arrange
        await ClearDatabaseAsync();
        var movieA = await CreateTestMovieAsync("A Movie");
        var movieB = await CreateTestMovieAsync("B Movie");
        var movieC = await CreateTestMovieAsync("C Movie");

        // Act
        var response = await Client.GetAsync("/api/movies?sortBy=title&sortOrder=asc");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var movieList = JsonSerializer.Deserialize<MovieListDto>(content, JsonOptions);

        movieList.Should().NotBeNull();
        movieList!.Movies.Should().HaveCount(3);
        movieList.Movies.First().Title.Should().Be("A Movie");
        movieList.Movies.Last().Title.Should().Be("C Movie");
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(1, 0)]
    [InlineData(1, 51)]
    public async Task GetMovies_WithInvalidPagination_ShouldReturnBadRequest(int page, int pageSize)
    {
        // Arrange
        await ClearDatabaseAsync();

        // Act
        var response = await Client.GetAsync($"/api/movies?page={page}&pageSize={pageSize}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SearchMovies_WithValidRequest_ShouldReturnResults()
    {
        // Arrange
        await ClearDatabaseAsync();
        await CreateTestMoviesWithGenresAsync();

        var searchRequest = new MovieSearchRequest
        {
            Query = "Test",
            Page = 1,
            PageSize = 5,
            SortBy = "title",
            SortOrder = "asc"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/movies/search", searchRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var movieList = JsonSerializer.Deserialize<MovieListDto>(content, JsonOptions);

        movieList.Should().NotBeNull();
        movieList!.Movies.Should().NotBeEmpty();
        movieList.Page.Should().Be(1);
        movieList.PageSize.Should().Be(5);
    }

    [Fact]
    public async Task SearchMovies_WithInvalidRequest_ShouldReturnBadRequest()
    {
        // Arrange
        await ClearDatabaseAsync();

        var searchRequest = new MovieSearchRequest
        {
            Query = new string('a', 101), // Too long
            Year = 1800, // Invalid year
            Page = 0, // Invalid page
            PageSize = 100 // Too large
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/movies/search", searchRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetMovie_WithValidId_ShouldReturnMovieDetails()
    {
        // Arrange
        await ClearDatabaseAsync();
        var movie = await CreateTestMovieWithDetailsAsync();

        // Act
        var response = await Client.GetAsync($"/api/movies/{movie.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var movieDto = JsonSerializer.Deserialize<MovieDto>(content, JsonOptions);

        movieDto.Should().NotBeNull();
        movieDto!.Id.Should().Be(movie.Id);
        movieDto.Title.Should().Be(movie.Title);
        movieDto.Genres.Should().NotBeEmpty();
        movieDto.Cast.Should().NotBeEmpty();
        movieDto.Crew.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetMovie_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        await ClearDatabaseAsync();
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"/api/movies/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetGenres_ShouldReturnAllGenres()
    {
        // Arrange
        await ClearDatabaseAsync();
        await CreateTestGenreAsync("Action");
        await CreateTestGenreAsync("Comedy");
        await CreateTestGenreAsync("Drama");

        // Act
        var response = await Client.GetAsync("/api/movies/genres");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var genres = JsonSerializer.Deserialize<List<GenreDto>>(content, JsonOptions);

        genres.Should().NotBeNull();
        genres!.Should().HaveCount(3);
        genres.Should().Contain(g => g.Name == "Action");
        genres.Should().Contain(g => g.Name == "Comedy");
        genres.Should().Contain(g => g.Name == "Drama");
    }

    [Fact]
    public async Task GetPopularMovies_ShouldReturnPopularMoviesSortedByPopularity()
    {
        // Arrange
        await ClearDatabaseAsync();
        var movie1 = await CreateTestMovieAsync("Movie 1");
        movie1.Popularity = 50;
        var movie2 = await CreateTestMovieAsync("Movie 2");
        movie2.Popularity = 100;
        var movie3 = await CreateTestMovieAsync("Movie 3");
        movie3.Popularity = 75;
        await DbContext.SaveChangesAsync();

        // Act
        var response = await Client.GetAsync("/api/movies/popular");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var movieList = JsonSerializer.Deserialize<MovieListDto>(content, JsonOptions);

        movieList.Should().NotBeNull();
        movieList!.Movies.Should().HaveCount(3);
        // Should be sorted by popularity descending
        movieList.Movies.First().Title.Should().Be("Movie 2");
        movieList.Movies.Last().Title.Should().Be("Movie 1");
    }

    [Fact]
    public async Task GetTopRatedMovies_ShouldReturnTopRatedMoviesSortedByRating()
    {
        // Arrange
        await ClearDatabaseAsync();
        var movie1 = await CreateTestMovieAsync("Movie 1");
        movie1.VoteAverage = 5.0m;
        var movie2 = await CreateTestMovieAsync("Movie 2");
        movie2.VoteAverage = 9.0m;
        var movie3 = await CreateTestMovieAsync("Movie 3");
        movie3.VoteAverage = 7.5m;
        await DbContext.SaveChangesAsync();

        // Act
        var response = await Client.GetAsync("/api/movies/top-rated");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var movieList = JsonSerializer.Deserialize<MovieListDto>(content, JsonOptions);

        movieList.Should().NotBeNull();
        movieList!.Movies.Should().HaveCount(3);
        // Should be sorted by vote average descending
        movieList.Movies.First().Title.Should().Be("Movie 2");
        movieList.Movies.Last().Title.Should().Be("Movie 1");
    }

    [Fact]
    public async Task GetUpcomingMovies_ShouldReturnFutureMovies()
    {
        // Arrange
        await ClearDatabaseAsync();
        var pastMovie = await CreateTestMovieAsync("Past Movie");
        pastMovie.ReleaseDate = DateTime.UtcNow.AddDays(-30);

        var currentMovie = await CreateTestMovieAsync("Current Movie");
        currentMovie.ReleaseDate = DateTime.UtcNow.AddDays(-1);

        var futureMovie = await CreateTestMovieAsync("Future Movie");
        futureMovie.ReleaseDate = DateTime.UtcNow.AddDays(30);

        await DbContext.SaveChangesAsync();
        RefreshDbContext();

        // Act
        var response = await Client.GetAsync("/api/movies/upcoming");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var movieList = JsonSerializer.Deserialize<MovieListDto>(content, JsonOptions);

        movieList.Should().NotBeNull();
        movieList!.Movies.Should().HaveCount(1);
        movieList.Movies.First().Title.Should().Be("Future Movie");
    }

    [Fact]
    public async Task GetMovies_WithPaginationBeyondLastPage_ShouldReturnEmptyList()
    {
        // Arrange
        await ClearDatabaseAsync();
        await CreateTestMoviesWithGenresAsync(5); // Only 5 movies

        // Act
        var response = await Client.GetAsync("/api/movies?page=10&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var movieList = JsonSerializer.Deserialize<MovieListDto>(content, JsonOptions);

        movieList.Should().NotBeNull();
        movieList!.Movies.Should().BeEmpty();
        movieList.TotalCount.Should().Be(5);
        movieList.Page.Should().Be(10);
        movieList.TotalPages.Should().Be(1);
    }

    private async Task<List<Movie>> CreateTestMoviesWithGenresAsync(int count = 5)
    {
        var genre = await CreateTestGenreAsync("Action");
        var movies = new List<Movie>();

        for (int i = 1; i <= count; i++)
        {
            var movie = await CreateTestMovieAsync($"Test Movie {i}");
            movie.Popularity = i * 10;
            movie.VoteAverage = i * 1.5m;
            
            DbContext.MovieGenres.Add(new MovieGenre 
            { 
                MovieId = movie.Id, 
                GenreId = genre.Id,
                Genre = genre
            });
            
            movies.Add(movie);
        }

        await DbContext.SaveChangesAsync();
        return movies;
    }

    private async Task<Movie> CreateTestMovieWithDetailsAsync()
    {
        var movie = await CreateTestMovieAsync("Detailed Test Movie");
        var genre = await CreateTestGenreAsync("Action");
        
        // Add genre
        DbContext.MovieGenres.Add(new MovieGenre 
        { 
            MovieId = movie.Id, 
            GenreId = genre.Id,
            Genre = genre
        });

        // Add cast
        var actor = new Person
        {
            Id = Guid.NewGuid(),
            Name = "Test Actor",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        DbContext.Persons.Add(actor);

        DbContext.MovieCasts.Add(new MovieCast
        {
            Id = Guid.NewGuid(),
            MovieId = movie.Id,
            PersonId = actor.Id,
            Character = "Hero",
            Order = 0,
            Person = actor,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        // Add crew
        var director = new Person
        {
            Id = Guid.NewGuid(),
            Name = "Test Director",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        DbContext.Persons.Add(director);

        DbContext.MovieCrews.Add(new MovieCrew
        {
            Id = Guid.NewGuid(),
            MovieId = movie.Id,
            PersonId = director.Id,
            Job = "Director",
            Department = "Directing",
            Person = director,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        await DbContext.SaveChangesAsync();
        return movie;
    }
}