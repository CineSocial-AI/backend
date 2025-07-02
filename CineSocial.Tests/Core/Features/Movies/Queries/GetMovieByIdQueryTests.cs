using CineSocial.Core.Features.Movies.Queries;
using CineSocial.Core.Shared.Interfaces;
using CineSocial.Domain.Entities;
using CineSocial.Tests.Helpers;
using FluentAssertions;
using Moq;

namespace CineSocial.Tests.Core.Features.Movies.Queries;

public class GetMovieByIdQueryTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IRepository<Movie>> _movieRepositoryMock;
    private readonly Mock<IRepository<Genre>> _genreRepositoryMock;
    private readonly Mock<IRepository<Person>> _personRepositoryMock;
    private readonly GetMovieByIdQueryHandler _handler;

    public GetMovieByIdQueryTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _movieRepositoryMock = new Mock<IRepository<Movie>>();
        _genreRepositoryMock = new Mock<IRepository<Genre>>();
        _personRepositoryMock = new Mock<IRepository<Person>>();
        
        _unitOfWorkMock.Setup(x => x.Movies).Returns(_movieRepositoryMock.Object);
        _unitOfWorkMock.Setup(x => x.Genres).Returns(_genreRepositoryMock.Object);
        _unitOfWorkMock.Setup(x => x.Persons).Returns(_personRepositoryMock.Object);
        
        _handler = new GetMovieByIdQueryHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_MovieExists_ShouldReturnSuccessResult()
    {
        // Arrange
        var movieId = Guid.NewGuid();
        var movie = CreateMovieWithDetails(movieId);
        var query = new GetMovieByIdQuery(movieId);

        _movieRepositoryMock.Setup(x => x.GetByIdAsync(
            movieId,
            It.IsAny<System.Linq.Expressions.Expression<Func<Movie, object>>[]>()))
            .ReturnsAsync(movie);

        // Setup genre and person mocks
        SetupGenreAndPersonMocks(movie);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(movieId);
        result.Data.Title.Should().Be(movie.Title);
        result.Data.Overview.Should().Be(movie.Overview);
        result.Data.Runtime.Should().Be(movie.Runtime ?? 0);
        result.Data.VoteAverage.Should().Be(movie.VoteAverage ?? 0);
        result.Data.Genres.Should().HaveCount(2);
        result.Data.Cast.Should().HaveCount(2);
        result.Data.Crew.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_MovieNotFound_ShouldReturnFailureResult()
    {
        // Arrange
        var movieId = Guid.NewGuid();
        var query = new GetMovieByIdQuery(movieId);

        _movieRepositoryMock.Setup(x => x.GetByIdAsync(
            movieId,
            It.IsAny<System.Linq.Expressions.Expression<Func<Movie, object>>[]>()))
            .ReturnsAsync((Movie?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Film bulunamadı.");
    }

    [Fact]
    public async Task Handle_MovieWithoutGenresAndCast_ShouldReturnEmptyCollections()
    {
        // Arrange
        var movieId = Guid.NewGuid();
        var movie = TestHelper.CreateTestMovie("Test Movie", movieId);
        var query = new GetMovieByIdQuery(movieId);

        _movieRepositoryMock.Setup(x => x.GetByIdAsync(
            movieId,
            It.IsAny<System.Linq.Expressions.Expression<Func<Movie, object>>[]>()))
            .ReturnsAsync(movie);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Genres.Should().BeEmpty();
        result.Data.Cast.Should().BeEmpty();
        result.Data.Crew.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_CastOrderedCorrectly_ShouldReturnOrderedCast()
    {
        // Arrange
        var movieId = Guid.NewGuid();
        var movie = CreateMovieWithOrderedCast(movieId);
        var query = new GetMovieByIdQuery(movieId);

        _movieRepositoryMock.Setup(x => x.GetByIdAsync(
            movieId,
            It.IsAny<System.Linq.Expressions.Expression<Func<Movie, object>>[]>()))
            .ReturnsAsync(movie);

        // Setup person mocks for cast
        SetupGenreAndPersonMocks(movie);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Cast.Should().HaveCount(3);
        result.Data.Cast[0].Order.Should().Be(0);
        result.Data.Cast[1].Order.Should().Be(1);
        result.Data.Cast[2].Order.Should().Be(2);
        result.Data.Cast[0].Name.Should().Be("Actor C");
        result.Data.Cast[1].Name.Should().Be("Actor A");
        result.Data.Cast[2].Name.Should().Be("Actor B");
    }

    private static Movie CreateMovieWithDetails(Guid movieId)
    {
        var movie = TestHelper.CreateTestMovie("Test Movie", movieId);
        movie.OriginalTitle = "Original Test Movie";
        movie.Language = "en";
        movie.Popularity = 85.5m;
        movie.Status = "Released";
        movie.Budget = 10000000;
        movie.Revenue = 50000000;
        movie.Tagline = "A great test movie";

        // Add genres
        var genre1 = new Genre { Id = Guid.NewGuid(), Name = "Action", Description = "Action movies" };
        var genre2 = new Genre { Id = Guid.NewGuid(), Name = "Drama", Description = "Drama movies" };
        
        movie.MovieGenres = new List<MovieGenre>
        {
            new() { MovieId = movieId, GenreId = genre1.Id, Genre = genre1 },
            new() { MovieId = movieId, GenreId = genre2.Id, Genre = genre2 }
        };

        // Add cast
        var person1 = new Person { Id = Guid.NewGuid(), Name = "John Doe" };
        var person2 = new Person { Id = Guid.NewGuid(), Name = "Jane Smith" };
        
        movie.MovieCasts = new List<MovieCast>
        {
            new() { MovieId = movieId, PersonId = person1.Id, Person = person1, Character = "Hero", Order = 0 },
            new() { MovieId = movieId, PersonId = person2.Id, Person = person2, Character = "Villain", Order = 1 }
        };

        // Add crew
        var director = new Person { Id = Guid.NewGuid(), Name = "Director Name" };
        
        movie.MovieCrews = new List<MovieCrew>
        {
            new() { MovieId = movieId, PersonId = director.Id, Person = director, Job = "Director", Department = "Directing" }
        };

        return movie;
    }

    private static Movie CreateMovieWithOrderedCast(Guid movieId)
    {
        var movie = TestHelper.CreateTestMovie("Test Movie", movieId);
        
        var personA = new Person { Id = Guid.NewGuid(), Name = "Actor A" };
        var personB = new Person { Id = Guid.NewGuid(), Name = "Actor B" };
        var personC = new Person { Id = Guid.NewGuid(), Name = "Actor C" };
        
        // Add cast in different order to test ordering
        movie.MovieCasts = new List<MovieCast>
        {
            new() { MovieId = movieId, PersonId = personA.Id, Person = personA, Character = "Second", Order = 1 },
            new() { MovieId = movieId, PersonId = personB.Id, Person = personB, Character = "Third", Order = 2 },
            new() { MovieId = movieId, PersonId = personC.Id, Person = personC, Character = "First", Order = 0 }
        };

        return movie;
    }

    private void SetupGenreAndPersonMocks(Movie movie)
    {
        // Setup genre mocks
        if (movie.MovieGenres != null)
        {
            foreach (var movieGenre in movie.MovieGenres)
            {
                if (movieGenre.Genre != null)
                {
                    _genreRepositoryMock.Setup(x => x.GetByIdAsync(movieGenre.GenreId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(movieGenre.Genre);
                }
            }
        }

        // Setup person mocks for cast
        if (movie.MovieCasts != null)
        {
            foreach (var movieCast in movie.MovieCasts)
            {
                if (movieCast.Person != null)
                {
                    _personRepositoryMock.Setup(x => x.GetByIdAsync(movieCast.PersonId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(movieCast.Person);
                }
            }
        }

        // Setup person mocks for crew
        if (movie.MovieCrews != null)
        {
            foreach (var movieCrew in movie.MovieCrews)
            {
                if (movieCrew.Person != null)
                {
                    _personRepositoryMock.Setup(x => x.GetByIdAsync(movieCrew.PersonId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(movieCrew.Person);
                }
            }
        }
    }
}