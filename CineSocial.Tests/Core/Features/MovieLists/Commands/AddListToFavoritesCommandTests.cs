using CineSocial.Core.Features.MovieLists.Commands;
using CineSocial.Core.Shared.Interfaces;
using CineSocial.Domain.Entities;
using FluentAssertions;
using NSubstitute;

namespace CineSocial.Tests.Core.Features.MovieLists.Commands;

public class AddListToFavoritesCommandTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<MovieList> _movieListRepository;
    private readonly IRepository<ListFavorite> _listFavoriteRepository;
    private readonly AddListToFavoritesCommandHandler _handler;

    public AddListToFavoritesCommandTests()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _movieListRepository = Substitute.For<IRepository<MovieList>>();
        _listFavoriteRepository = Substitute.For<IRepository<ListFavorite>>();
        
        _unitOfWork.MovieLists.Returns(_movieListRepository);
        _unitOfWork.ListFavorites.Returns(_listFavoriteRepository);
        
        _handler = new AddListToFavoritesCommandHandler(_unitOfWork);
    }

    [Fact]
    public async Task Handle_WithValidPublicList_ShouldAddToFavorites()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var listOwnerId = Guid.NewGuid();
        var movieListId = Guid.NewGuid();

        var movieList = new MovieList
        {
            Id = movieListId,
            UserId = listOwnerId,
            Name = "Public List",
            IsPublic = true
        };

        var command = new AddListToFavoritesCommand(userId, movieListId);

        _movieListRepository.GetByIdAsync(movieListId, Arg.Any<CancellationToken>())
            .Returns(movieList);

        _listFavoriteRepository.FirstOrDefaultAsync(
            Arg.Any<System.Linq.Expressions.Expression<Func<ListFavorite, bool>>>(),
            Arg.Any<CancellationToken>())
            .Returns((ListFavorite?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeTrue();

        await _listFavoriteRepository.Received(1).AddAsync(
            Arg.Is<ListFavorite>(lf => 
                lf.UserId == userId && 
                lf.MovieListId == movieListId),
            Arg.Any<CancellationToken>());
        
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithOwnPublicList_ShouldAddToFavorites()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var movieListId = Guid.NewGuid();

        var movieList = new MovieList
        {
            Id = movieListId,
            UserId = userId, // User's own list
            Name = "My Public List",
            IsPublic = true
        };

        var command = new AddListToFavoritesCommand(userId, movieListId);

        _movieListRepository.GetByIdAsync(movieListId, Arg.Any<CancellationToken>())
            .Returns(movieList);

        _listFavoriteRepository.FirstOrDefaultAsync(
            Arg.Any<System.Linq.Expressions.Expression<Func<ListFavorite, bool>>>(),
            Arg.Any<CancellationToken>())
            .Returns((ListFavorite?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeTrue();

        await _listFavoriteRepository.Received(1).AddAsync(Arg.Any<ListFavorite>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithOwnPrivateList_ShouldAddToFavorites()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var movieListId = Guid.NewGuid();

        var movieList = new MovieList
        {
            Id = movieListId,
            UserId = userId, // User's own list
            Name = "My Private List",
            IsPublic = false
        };

        var command = new AddListToFavoritesCommand(userId, movieListId);

        _movieListRepository.GetByIdAsync(movieListId, Arg.Any<CancellationToken>())
            .Returns(movieList);

        _listFavoriteRepository.FirstOrDefaultAsync(
            Arg.Any<System.Linq.Expressions.Expression<Func<ListFavorite, bool>>>(),
            Arg.Any<CancellationToken>())
            .Returns((ListFavorite?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeTrue();

        await _listFavoriteRepository.Received(1).AddAsync(Arg.Any<ListFavorite>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithNonExistentList_ShouldReturnFailure()
    {
        // Arrange
        var command = new AddListToFavoritesCommand(Guid.NewGuid(), Guid.NewGuid());

        _movieListRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((MovieList?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Liste bulunamadı.");
        
        await _listFavoriteRepository.DidNotReceive().AddAsync(Arg.Any<ListFavorite>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithOthersPrivateList_ShouldReturnFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var listOwnerId = Guid.NewGuid();
        var movieListId = Guid.NewGuid();

        var movieList = new MovieList
        {
            Id = movieListId,
            UserId = listOwnerId, // Different user
            Name = "Private List",
            IsPublic = false
        };

        var command = new AddListToFavoritesCommand(userId, movieListId);

        _movieListRepository.GetByIdAsync(movieListId, Arg.Any<CancellationToken>())
            .Returns(movieList);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Bu listeyi favorilere ekleyemezsiniz.");
        
        await _listFavoriteRepository.DidNotReceive().AddAsync(Arg.Any<ListFavorite>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithAlreadyFavoritedList_ShouldReturnFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var listOwnerId = Guid.NewGuid();
        var movieListId = Guid.NewGuid();

        var movieList = new MovieList
        {
            Id = movieListId,
            UserId = listOwnerId,
            Name = "Public List",
            IsPublic = true
        };

        var existingFavorite = new ListFavorite
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            MovieListId = movieListId
        };

        var command = new AddListToFavoritesCommand(userId, movieListId);

        _movieListRepository.GetByIdAsync(movieListId, Arg.Any<CancellationToken>())
            .Returns(movieList);

        _listFavoriteRepository.FirstOrDefaultAsync(
            Arg.Any<System.Linq.Expressions.Expression<Func<ListFavorite, bool>>>(),
            Arg.Any<CancellationToken>())
            .Returns(existingFavorite);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Bu liste zaten favorilerinizde.");
        
        await _listFavoriteRepository.DidNotReceive().AddAsync(Arg.Any<ListFavorite>(), Arg.Any<CancellationToken>());
    }
}