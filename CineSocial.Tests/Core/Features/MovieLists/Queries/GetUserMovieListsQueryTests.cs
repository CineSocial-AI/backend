using CineSocial.Core.Features.MovieLists.Queries;
using CineSocial.Core.Shared.Interfaces;
using CineSocial.Domain.Entities;
using FluentAssertions;
using NSubstitute;

namespace CineSocial.Tests.Core.Features.MovieLists.Queries;

public class GetUserMovieListsQueryTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly IRepository<MovieList> _movieListRepository;
    private readonly IRepository<MovieListItem> _movieListItemRepository;
    private readonly GetUserMovieListsQueryHandler _handler;

    public GetUserMovieListsQueryTests()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _userRepository = Substitute.For<IUserRepository>();
        _movieListRepository = Substitute.For<IRepository<MovieList>>();
        _movieListItemRepository = Substitute.For<IRepository<MovieListItem>>();
        
        _unitOfWork.Users.Returns(_userRepository);
        _unitOfWork.MovieLists.Returns(_movieListRepository);
        _unitOfWork.MovieListItems.Returns(_movieListItemRepository);
        
        _handler = new GetUserMovieListsQueryHandler(_unitOfWork);
    }

    [Fact]
    public async Task Handle_WithValidUser_ShouldReturnUserLists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            FirstName = "John",
            LastName = "Doe",
            Username = "johndoe"
        };

        var watchlist = new MovieList
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = "Watchlist",
            Description = "My watchlist",
            IsWatchlist = true,
            IsPublic = false,
            CreatedAt = DateTime.UtcNow.AddDays(-10)
        };

        var customList = new MovieList
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = "Favorites",
            Description = "My favorite movies",
            IsWatchlist = false,
            IsPublic = true,
            CreatedAt = DateTime.UtcNow.AddDays(-5)
        };

        var movieLists = new[] { watchlist, customList };

        var movieListItems = new[]
        {
            new MovieListItem { MovieListId = watchlist.Id },
            new MovieListItem { MovieListId = watchlist.Id },
            new MovieListItem { MovieListId = customList.Id }
        };

        var query = new GetUserMovieListsQuery(userId, true);

        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(user);

        _movieListRepository.FindAsync(
            Arg.Any<System.Linq.Expressions.Expression<Func<MovieList, bool>>>(),
            Arg.Any<CancellationToken>())
            .Returns(movieLists);

        _movieListItemRepository.FindAsync(
            Arg.Any<System.Linq.Expressions.Expression<Func<MovieListItem, bool>>>(),
            Arg.Any<CancellationToken>())
            .Returns(movieListItems);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Should().HaveCount(2);

        var watchlistResult = result.Data.First(l => l.IsWatchlist);
        watchlistResult.Name.Should().Be("Watchlist");
        watchlistResult.MovieCount.Should().Be(2);
        watchlistResult.IsPublic.Should().BeFalse();

        var customListResult = result.Data.First(l => !l.IsWatchlist);
        customListResult.Name.Should().Be("Favorites");
        customListResult.MovieCount.Should().Be(1);
        customListResult.IsPublic.Should().BeTrue();

        // Should be ordered by creation date
        result.Data.First().CreatedAt.Should().BeBefore(result.Data.Last().CreatedAt);
    }

    [Fact]
    public async Task Handle_WithIncludeWatchlistFalse_ShouldExcludeWatchlist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            FirstName = "John",
            LastName = "Doe",
            Username = "johndoe"
        };

        var watchlist = new MovieList
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = "Watchlist",
            IsWatchlist = true,
            IsPublic = false
        };

        var customList = new MovieList
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = "Favorites",
            IsWatchlist = false,
            IsPublic = true
        };

        var movieLists = new[] { customList }; // Only non-watchlist

        var query = new GetUserMovieListsQuery(userId, false);

        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(user);

        _movieListRepository.FindAsync(
            Arg.Any<System.Linq.Expressions.Expression<Func<MovieList, bool>>>(),
            Arg.Any<CancellationToken>())
            .Returns(movieLists);

        _movieListItemRepository.FindAsync(
            Arg.Any<System.Linq.Expressions.Expression<Func<MovieListItem, bool>>>(),
            Arg.Any<CancellationToken>())
            .Returns(new List<MovieListItem>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Should().HaveCount(1);
        result.Data.First().Name.Should().Be("Favorites");
        result.Data.First().IsWatchlist.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WithNonExistentUser_ShouldReturnFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetUserMovieListsQuery(userId, true);

        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns((User?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Kullanıcı bulunamadı.");
        
        await _movieListRepository.DidNotReceive().FindAsync(
            Arg.Any<System.Linq.Expressions.Expression<Func<MovieList, bool>>>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithEmptyLists_ShouldReturnEmptyResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            FirstName = "John",
            LastName = "Doe",
            Username = "johndoe"
        };

        var query = new GetUserMovieListsQuery(userId, true);

        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(user);

        _movieListRepository.FindAsync(
            Arg.Any<System.Linq.Expressions.Expression<Func<MovieList, bool>>>(),
            Arg.Any<CancellationToken>())
            .Returns(new List<MovieList>());

        _movieListItemRepository.FindAsync(
            Arg.Any<System.Linq.Expressions.Expression<Func<MovieListItem, bool>>>(),
            Arg.Any<CancellationToken>())
            .Returns(new List<MovieListItem>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldCalculateMovieCountsCorrectly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            FirstName = "John",
            LastName = "Doe",
            Username = "johndoe"
        };

        var list1Id = Guid.NewGuid();
        var list2Id = Guid.NewGuid();

        var list1 = new MovieList
        {
            Id = list1Id,
            UserId = userId,
            Name = "List 1",
            IsWatchlist = false,
            IsPublic = true
        };

        var list2 = new MovieList
        {
            Id = list2Id,
            UserId = userId,
            Name = "List 2",
            IsWatchlist = false,
            IsPublic = true
        };

        var movieLists = new[] { list1, list2 };

        var movieListItems = new[]
        {
            new MovieListItem { MovieListId = list1Id },
            new MovieListItem { MovieListId = list1Id },
            new MovieListItem { MovieListId = list1Id },
            new MovieListItem { MovieListId = list2Id }
        };

        var query = new GetUserMovieListsQuery(userId, true);

        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(user);

        _movieListRepository.FindAsync(
            Arg.Any<System.Linq.Expressions.Expression<Func<MovieList, bool>>>(),
            Arg.Any<CancellationToken>())
            .Returns(movieLists);

        _movieListItemRepository.FindAsync(
            Arg.Any<System.Linq.Expressions.Expression<Func<MovieListItem, bool>>>(),
            Arg.Any<CancellationToken>())
            .Returns(movieListItems);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Should().HaveCount(2);

        var firstList = result.Data.First(l => l.Name == "List 1");
        var secondList = result.Data.First(l => l.Name == "List 2");

        firstList.MovieCount.Should().Be(3);
        secondList.MovieCount.Should().Be(1);
    }

    [Fact]
    public async Task Handle_ShouldReturnCorrectUserInformation()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            FirstName = "John",
            LastName = "Doe",
            Username = "johndoe"
        };

        var movieList = new MovieList
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = "Test List",
            Description = "Test Description",
            IsWatchlist = false,
            IsPublic = true,
            CreatedAt = new DateTime(2024, 1, 1),
            UpdatedAt = new DateTime(2024, 2, 1)
        };

        var query = new GetUserMovieListsQuery(userId, true);

        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(user);

        _movieListRepository.FindAsync(
            Arg.Any<System.Linq.Expressions.Expression<Func<MovieList, bool>>>(),
            Arg.Any<CancellationToken>())
            .Returns(new[] { movieList });

        _movieListItemRepository.FindAsync(
            Arg.Any<System.Linq.Expressions.Expression<Func<MovieListItem, bool>>>(),
            Arg.Any<CancellationToken>())
            .Returns(new List<MovieListItem>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Should().HaveCount(1);

        var listResult = result.Data.First();
        listResult.UserFullName.Should().Be("John Doe");
        listResult.UserUsername.Should().Be("johndoe");
        listResult.Name.Should().Be("Test List");
        listResult.Description.Should().Be("Test Description");
        listResult.CreatedAt.Should().Be(new DateTime(2024, 1, 1));
        listResult.UpdatedAt.Should().Be(new DateTime(2024, 2, 1));
    }
}