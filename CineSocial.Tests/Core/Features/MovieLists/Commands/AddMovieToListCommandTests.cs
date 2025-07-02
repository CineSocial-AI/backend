using CineSocial.Core.Features.MovieLists.Commands;
using CineSocial.Core.Shared.Interfaces;
using CineSocial.Domain.Entities;
using FluentAssertions;
using NSubstitute;

namespace CineSocial.Tests.Core.Features.MovieLists.Commands;

public class AddMovieToListCommandTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<MovieList> _movieListRepository;
    private readonly IRepository<Movie> _movieRepository;
    private readonly IRepository<MovieListItem> _movieListItemRepository;
    private readonly AddMovieToListCommandHandler _handler;

    public AddMovieToListCommandTests()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _movieListRepository = Substitute.For<IRepository<MovieList>>();
        _movieRepository = Substitute.For<IRepository<Movie>>();
        _movieListItemRepository = Substitute.For<IRepository<MovieListItem>>();
        
        _unitOfWork.MovieLists.Returns(_movieListRepository);
        _unitOfWork.Movies.Returns(_movieRepository);
        _unitOfWork.MovieListItems.Returns(_movieListItemRepository);
        
        _handler = new AddMovieToListCommandHandler(_unitOfWork);
    }

    [Fact]
    public async Task Handle_WithValidData_ShouldAddMovieToList()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var movieListId = Guid.NewGuid();
        var movieId = Guid.NewGuid();

        var movieList = new MovieList
        {
            Id = movieListId,
            UserId = userId,
            Name = "Test List"
        };

        var movie = new Movie
        {
            Id = movieId,
            Title = "Test Movie",
            PosterPath = "/test-poster.jpg"
        };

        var command = new AddMovieToListCommand(userId, movieListId, movieId, "Great movie!");

        _movieListRepository.GetByIdAsync(movieListId, Arg.Any<CancellationToken>())
            .Returns(movieList);

        _movieRepository.GetByIdAsync(movieId, Arg.Any<CancellationToken>())
            .Returns(movie);

        _movieListItemRepository.FirstOrDefaultAsync(
            Arg.Any<System.Linq.Expressions.Expression<Func<MovieListItem, bool>>>(),
            Arg.Any<CancellationToken>())
            .Returns((MovieListItem?)null);

        _movieListItemRepository.FindAsync(
            Arg.Any<System.Linq.Expressions.Expression<Func<MovieListItem, bool>>>(),
            Arg.Any<CancellationToken>())
            .Returns(new List<MovieListItem>());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.MovieId.Should().Be(movieId);
        result.Data.MovieTitle.Should().Be("Test Movie");
        result.Data.MoviePosterPath.Should().Be("/test-poster.jpg");
        result.Data.Notes.Should().Be("Great movie!");
        result.Data.Order.Should().Be(1);

        await _movieListItemRepository.Received(1).AddAsync(
            Arg.Is<MovieListItem>(mli => 
                mli.MovieListId == movieListId && 
                mli.MovieId == movieId &&
                mli.Notes == "Great movie!" &&
                mli.Order == 1),
            Arg.Any<CancellationToken>());
        
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithNonExistentMovieList_ShouldReturnFailure()
    {
        // Arrange
        var command = new AddMovieToListCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Notes");

        _movieListRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((MovieList?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Liste bulunamadı.");
        
        await _movieListItemRepository.DidNotReceive().AddAsync(Arg.Any<MovieListItem>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithUnauthorizedUser_ShouldReturnFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var movieListId = Guid.NewGuid();
        var movieId = Guid.NewGuid();

        var movieList = new MovieList
        {
            Id = movieListId,
            UserId = otherUserId, // Different user
            Name = "Other User's List"
        };

        var command = new AddMovieToListCommand(userId, movieListId, movieId, "Notes");

        _movieListRepository.GetByIdAsync(movieListId, Arg.Any<CancellationToken>())
            .Returns(movieList);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Bu listeye film ekleme yetkiniz yok.");
        
        await _movieRepository.DidNotReceive().GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        await _movieListItemRepository.DidNotReceive().AddAsync(Arg.Any<MovieListItem>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithNonExistentMovie_ShouldReturnFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var movieListId = Guid.NewGuid();
        var movieId = Guid.NewGuid();

        var movieList = new MovieList
        {
            Id = movieListId,
            UserId = userId,
            Name = "Test List"
        };

        var command = new AddMovieToListCommand(userId, movieListId, movieId, "Notes");

        _movieListRepository.GetByIdAsync(movieListId, Arg.Any<CancellationToken>())
            .Returns(movieList);

        _movieRepository.GetByIdAsync(movieId, Arg.Any<CancellationToken>())
            .Returns((Movie?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Film bulunamadı.");
        
        await _movieListItemRepository.DidNotReceive().AddAsync(Arg.Any<MovieListItem>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithDuplicateMovie_ShouldReturnFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var movieListId = Guid.NewGuid();
        var movieId = Guid.NewGuid();

        var movieList = new MovieList
        {
            Id = movieListId,
            UserId = userId,
            Name = "Test List"
        };

        var movie = new Movie
        {
            Id = movieId,
            Title = "Test Movie"
        };

        var existingItem = new MovieListItem
        {
            Id = Guid.NewGuid(),
            MovieListId = movieListId,
            MovieId = movieId
        };

        var command = new AddMovieToListCommand(userId, movieListId, movieId, "Notes");

        _movieListRepository.GetByIdAsync(movieListId, Arg.Any<CancellationToken>())
            .Returns(movieList);

        _movieRepository.GetByIdAsync(movieId, Arg.Any<CancellationToken>())
            .Returns(movie);

        _movieListItemRepository.FirstOrDefaultAsync(
            Arg.Any<System.Linq.Expressions.Expression<Func<MovieListItem, bool>>>(),
            Arg.Any<CancellationToken>())
            .Returns(existingItem);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Bu film zaten listede mevcut.");
        
        await _movieListItemRepository.DidNotReceive().AddAsync(Arg.Any<MovieListItem>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithExistingMovies_ShouldCalculateCorrectOrder()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var movieListId = Guid.NewGuid();
        var movieId = Guid.NewGuid();

        var movieList = new MovieList
        {
            Id = movieListId,
            UserId = userId,
            Name = "Test List"
        };

        var movie = new Movie
        {
            Id = movieId,
            Title = "Test Movie",
            PosterPath = "/test-poster.jpg"
        };

        var existingItems = new List<MovieListItem>
        {
            new MovieListItem { Order = 1 },
            new MovieListItem { Order = 3 },
            new MovieListItem { Order = 2 }
        };

        var command = new AddMovieToListCommand(userId, movieListId, movieId, "Notes");

        _movieListRepository.GetByIdAsync(movieListId, Arg.Any<CancellationToken>())
            .Returns(movieList);

        _movieRepository.GetByIdAsync(movieId, Arg.Any<CancellationToken>())
            .Returns(movie);

        _movieListItemRepository.FirstOrDefaultAsync(
            Arg.Any<System.Linq.Expressions.Expression<Func<MovieListItem, bool>>>(),
            Arg.Any<CancellationToken>())
            .Returns((MovieListItem?)null);

        _movieListItemRepository.FindAsync(
            Arg.Any<System.Linq.Expressions.Expression<Func<MovieListItem, bool>>>(),
            Arg.Any<CancellationToken>())
            .Returns(existingItems);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Order.Should().Be(4); // Max order (3) + 1

        await _movieListItemRepository.Received(1).AddAsync(
            Arg.Is<MovieListItem>(mli => mli.Order == 4),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithEmptyNotes_ShouldUseEmptyString()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var movieListId = Guid.NewGuid();
        var movieId = Guid.NewGuid();

        var movieList = new MovieList
        {
            Id = movieListId,
            UserId = userId,
            Name = "Test List"
        };

        var movie = new Movie
        {
            Id = movieId,
            Title = "Test Movie",
            PosterPath = "/test-poster.jpg"
        };

        var command = new AddMovieToListCommand(userId, movieListId, movieId, null);

        _movieListRepository.GetByIdAsync(movieListId, Arg.Any<CancellationToken>())
            .Returns(movieList);

        _movieRepository.GetByIdAsync(movieId, Arg.Any<CancellationToken>())
            .Returns(movie);

        _movieListItemRepository.FirstOrDefaultAsync(
            Arg.Any<System.Linq.Expressions.Expression<Func<MovieListItem, bool>>>(),
            Arg.Any<CancellationToken>())
            .Returns((MovieListItem?)null);

        _movieListItemRepository.FindAsync(
            Arg.Any<System.Linq.Expressions.Expression<Func<MovieListItem, bool>>>(),
            Arg.Any<CancellationToken>())
            .Returns(new List<MovieListItem>());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Notes.Should().Be("");

        await _movieListItemRepository.Received(1).AddAsync(
            Arg.Is<MovieListItem>(mli => mli.Notes == ""),
            Arg.Any<CancellationToken>());
    }
}