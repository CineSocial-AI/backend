using CineSocial.Api.DTOs;
using Swashbuckle.AspNetCore.Filters;

namespace CineSocial.Api.Swagger.Examples;

public class CreateMovieListRequestExample : IExamplesProvider<CreateMovieListRequest>
{
    public CreateMovieListRequest GetExamples()
    {
        return new CreateMovieListRequest
        {
            Name = "Favori Sci-Fi Filmlerim",
            Description = "En sevdiğim bilim kurgu filmleri. Christopher Nolan'ın eserleri ağırlıkta.",
            IsPublic = true
        };
    }
}

public class UpdateMovieListRequestExample : IExamplesProvider<UpdateMovieListRequest>
{
    public UpdateMovieListRequest GetExamples()
    {
        return new UpdateMovieListRequest
        {
            Name = "En İyi Sci-Fi Filmlerim",
            Description = "En sevdiğim bilim kurgu filmleri. Güncellenmiş ve genişletilmiş liste.",
            IsPublic = false
        };
    }
}

public class AddMovieToListRequestExample : IExamplesProvider<AddMovieToListRequest>
{
    public AddMovieToListRequest GetExamples()
    {
        return new AddMovieToListRequest
        {
            MovieId = Guid.Parse("a1b2c3d4-e5f6-7890-1234-567890abcdef"),
            Notes = "Christopher Nolan'ın şaheseri. Mutlaka tekrar izlemeliyim."
        };
    }
}

public class UserMovieListDtoExample : IExamplesProvider<UserMovieListDto>
{
    public UserMovieListDto GetExamples()
    {
        return new UserMovieListDto
        {
            Id = Guid.Parse("11111111-2222-3333-4444-555555555555"),
            UserId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"),
            Name = "Favori Sci-Fi Filmlerim",
            Description = "En sevdiğim bilim kurgu filmleri. Christopher Nolan'ın eserleri ağırlıkta.",
            IsPublic = true,
            IsWatchlist = false,
            MovieCount = 5,
            CreatedAt = new DateTime(2024, 1, 15, 10, 30, 0),
            UpdatedAt = new DateTime(2024, 2, 20, 14, 45, 0),
            UserFullName = "Ahmet Yılmaz",
            UserUsername = "cinemafan01",
            IsFavorited = false
        };
    }
}

public class MovieListDetailDtoExample : IExamplesProvider<MovieListDetailDto>
{
    public MovieListDetailDto GetExamples()
    {
        return new MovieListDetailDto
        {
            Id = Guid.Parse("11111111-2222-3333-4444-555555555555"),
            UserId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"),
            Name = "Favori Sci-Fi Filmlerim",
            Description = "En sevdiğim bilim kurgu filmleri. Christopher Nolan'ın eserleri ağırlıkta.",
            IsPublic = true,
            IsWatchlist = false,
            Movies = new List<MovieListItemDto>
            {
                new MovieListItemDto
                {
                    Id = Guid.Parse("12345678-1234-1234-1234-123456789abc"),
                    MovieId = Guid.Parse("a1b2c3d4-e5f6-7890-1234-567890abcdef"),
                    MovieTitle = "Inception",
                    MoviePosterPath = "/inception_poster.jpg",
                    Notes = "Nolan'ın en iyi sci-fi eseri",
                    Order = 1,
                    AddedAt = new DateTime(2024, 1, 15, 11, 0, 0)
                },
                new MovieListItemDto
                {
                    Id = Guid.Parse("87654321-4321-4321-4321-cba987654321"),
                    MovieId = Guid.Parse("f1e2d3c4-b5a6-9876-5432-109876543210"),
                    MovieTitle = "Interstellar",
                    MoviePosterPath = "/interstellar_poster.jpg",
                    Notes = "Uzay ve zaman konusu muhteşem",
                    Order = 2,
                    AddedAt = new DateTime(2024, 1, 20, 16, 30, 0)
                }
            },
            CreatedAt = new DateTime(2024, 1, 15, 10, 30, 0),
            UpdatedAt = new DateTime(2024, 2, 20, 14, 45, 0),
            UserFullName = "Ahmet Yılmaz",
            UserUsername = "cinemafan01",
            IsFavorited = true
        };
    }
}

public class MovieListItemDtoExample : IExamplesProvider<MovieListItemDto>
{
    public MovieListItemDto GetExamples()
    {
        return new MovieListItemDto
        {
            Id = Guid.Parse("12345678-1234-1234-1234-123456789abc"),
            MovieId = Guid.Parse("a1b2c3d4-e5f6-7890-1234-567890abcdef"),
            MovieTitle = "Inception",
            MoviePosterPath = "/inception_poster.jpg",
            Notes = "Christopher Nolan'ın şaheseri. Mutlaka tekrar izlemeliyim.",
            Order = 1,
            AddedAt = new DateTime(2024, 2, 20, 15, 30, 0)
        };
    }
}

public class PagedUserMovieListDtoExample : IExamplesProvider<PagedUserMovieListDto>
{
    public PagedUserMovieListDto GetExamples()
    {
        return new PagedUserMovieListDto
        {
            MovieLists = new List<UserMovieListDto>
            {
                new UserMovieListDto
                {
                    Id = Guid.Parse("11111111-2222-3333-4444-555555555555"),
                    UserId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"),
                    Name = "2023'ün En İyileri",
                    Description = "2023 yılının en beğendiğim filmleri. Barbenheimer dönemi unutulmaz!",
                    IsPublic = true,
                    IsWatchlist = false,
                    MovieCount = 3,
                    CreatedAt = new DateTime(2023, 12, 1, 9, 0, 0),
                    UpdatedAt = new DateTime(2023, 12, 25, 18, 30, 0),
                    UserFullName = "Elif Kaya",
                    UserUsername = "moviecritic",
                    IsFavorited = false
                },
                new UserMovieListDto
                {
                    Id = Guid.Parse("22222222-3333-4444-5555-666666666666"),
                    UserId = Guid.Parse("bbbbbbbb-cccc-dddd-eeee-ffffffffffff"),
                    Name = "Aksiyon Dolu Filmler",
                    Description = "Nefes kesen aksiyon filmleri koleksiyonum. Adrenalin garantili!",
                    IsPublic = true,
                    IsWatchlist = false,
                    MovieCount = 7,
                    CreatedAt = new DateTime(2024, 1, 10, 14, 20, 0),
                    UpdatedAt = new DateTime(2024, 2, 15, 12, 45, 0),
                    UserFullName = "Mehmet Demir",
                    UserUsername = "blockbusterlover",
                    IsFavorited = true
                }
            },
            TotalCount = 15,
            Page = 1,
            PageSize = 2,
            TotalPages = 8
        };
    }
}

public class UserMovieListArrayExample : IExamplesProvider<List<UserMovieListDto>>
{
    public List<UserMovieListDto> GetExamples()
    {
        return new List<UserMovieListDto>
        {
            new UserMovieListDto
            {
                Id = Guid.Parse("11111111-2222-3333-4444-555555555555"),
                UserId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"),
                Name = "İzleme Listesi",
                Description = "İzlemeyi planladığım filmler",
                IsPublic = false,
                IsWatchlist = true,
                MovieCount = 12,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0),
                UpdatedAt = new DateTime(2024, 2, 28, 20, 15, 0),
                UserFullName = "Ahmet Yılmaz",
                UserUsername = "cinemafan01",
                IsFavorited = false
            },
            new UserMovieListDto
            {
                Id = Guid.Parse("22222222-3333-4444-5555-666666666666"),
                UserId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"),
                Name = "Favori Sci-Fi Filmlerim",
                Description = "En sevdiğim bilim kurgu filmleri. Christopher Nolan'ın eserleri ağırlıkta.",
                IsPublic = true,
                IsWatchlist = false,
                MovieCount = 5,
                CreatedAt = new DateTime(2024, 1, 15, 10, 30, 0),
                UpdatedAt = new DateTime(2024, 2, 20, 14, 45, 0),
                UserFullName = "Ahmet Yılmaz",
                UserUsername = "cinemafan01",
                IsFavorited = false
            },
            new UserMovieListDto
            {
                Id = Guid.Parse("33333333-4444-5555-6666-777777777777"),
                UserId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"),
                Name = "Zihin Açıcı Filmler",
                Description = "Düşündüren, analiz etmeye değer filmler",
                IsPublic = true,
                IsWatchlist = false,
                MovieCount = 8,
                CreatedAt = new DateTime(2024, 2, 1, 8, 15, 0),
                UpdatedAt = null,
                UserFullName = "Ahmet Yılmaz",
                UserUsername = "cinemafan01",
                IsFavorited = false
            }
        };
    }
}