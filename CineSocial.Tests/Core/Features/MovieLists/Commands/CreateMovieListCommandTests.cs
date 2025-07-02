using CineSocial.Core.Features.MovieLists.Commands;
using CineSocial.Core.Shared.Interfaces;
using CineSocial.Domain.Entities;
using FluentAssertions;
using NSubstitute;

namespace CineSocial.Tests.Core.Features.MovieLists.Commands;

public class CreateMovieListCommandTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly IRepository<MovieList> _movieListRepository;
    private readonly CreateMovieListCommandHandler _handler;

    public CreateMovieListCommandTests()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _userRepository = Substitute.For<IUserRepository>();
        _movieListRepository = Substitute.For<IRepository<MovieList>>();
        
        _unitOfWork.Users.Returns(_userRepository);
        _unitOfWork.MovieLists.Returns(_movieListRepository);
        
        _handler = new CreateMovieListCommandHandler(_unitOfWork);
    }

    [Fact]
    public async Task Handle_WithValidData_ShouldCreateMovieList()
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

        var command = new CreateMovieListCommand(
            userId,
            "My Favorite Movies",
            "A collection of my favorite films",
            true
        );

        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(user);

        _movieListRepository.FirstOrDefaultAsync(
            Arg.Any<System.Linq.Expressions.Expression<Func<MovieList, bool>>>(),
            Arg.Any<CancellationToken>())
            .Returns((MovieList?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("My Favorite Movies");
        result.Data.Description.Should().Be("A collection of my favorite films");
        result.Data.IsPublic.Should().BeTrue();
        result.Data.UserFullName.Should().Be("John Doe");
        result.Data.UserUsername.Should().Be("johndoe");
        result.Data.MovieCount.Should().Be(0);

        await _movieListRepository.Received(1).AddAsync(
            Arg.Is<MovieList>(ml => 
                ml.Name == "My Favorite Movies" && 
                ml.UserId == userId &&
                ml.IsPublic == true),
            Arg.Any<CancellationToken>());
        
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithNonExistentUser_ShouldReturnFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreateMovieListCommand(userId, "Test List", "Description", true);

        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns((User?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Kullanıcı bulunamadı.");
        
        await _movieListRepository.DidNotReceive().AddAsync(Arg.Any<MovieList>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithDuplicateListName_ShouldReturnFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, FirstName = "John", LastName = "Doe", Username = "johndoe" };
        var existingList = new MovieList { Id = Guid.NewGuid(), Name = "My Favorite Movies", UserId = userId };

        var command = new CreateMovieListCommand(userId, "My Favorite Movies", "Description", true);

        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(user);

        _movieListRepository.FirstOrDefaultAsync(
            Arg.Any<System.Linq.Expressions.Expression<Func<MovieList, bool>>>(),
            Arg.Any<CancellationToken>())
            .Returns(existingList);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Bu isimde bir listeniz zaten mevcut.");
        
        await _movieListRepository.DidNotReceive().AddAsync(Arg.Any<MovieList>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithPrivateList_ShouldCreatePrivateList()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            FirstName = "Jane",
            LastName = "Smith",
            Username = "janesmith"
        };

        var command = new CreateMovieListCommand(
            userId,
            "Private Collection",
            "My private movie collection",
            false
        );

        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(user);

        _movieListRepository.FirstOrDefaultAsync(
            Arg.Any<System.Linq.Expressions.Expression<Func<MovieList, bool>>>(),
            Arg.Any<CancellationToken>())
            .Returns((MovieList?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.IsPublic.Should().BeFalse();

        await _movieListRepository.Received(1).AddAsync(
            Arg.Is<MovieList>(ml => ml.IsPublic == false),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithEmptyDescription_ShouldUseEmptyString()
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

        var command = new CreateMovieListCommand(userId, "Test List", null, true);

        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(user);

        _movieListRepository.FirstOrDefaultAsync(
            Arg.Any<System.Linq.Expressions.Expression<Func<MovieList, bool>>>(),
            Arg.Any<CancellationToken>())
            .Returns((MovieList?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Description.Should().Be("");

        await _movieListRepository.Received(1).AddAsync(
            Arg.Is<MovieList>(ml => ml.Description == ""),
            Arg.Any<CancellationToken>());
    }
}