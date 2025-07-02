using CineSocial.Api.DTOs;
using CineSocial.Domain.Enums;
using Swashbuckle.AspNetCore.Filters;

namespace CineSocial.Api.Swagger.Examples;

public class ReviewDtoExample : IExamplesProvider<ReviewDto>
{
    public ReviewDto GetExamples()
    {
        return new ReviewDto
        {
            Id = Guid.Parse("550e8400-e29b-41d4-a716-446655440000"),
            UserId = Guid.Parse("660e8400-e29b-41d4-a716-446655440001"),
            MovieId = Guid.Parse("770e8400-e29b-41d4-a716-446655440002"),
            Title = "Zihinsel Bir Şaheser",
            Content = "Christopher Nolan'ın en karmaşık ve etkileyici filmlerinden biri. Rüya içinde rüya konsepti mükemmel işlenmiş. Leonardo DiCaprio harika bir performans sergilemiş. Görsel efektler nefes kesici, hikaye ise sizi baştan sona ekrana kilitliyor.",
            ContainsSpoilers = false,
            LikesCount = 15,
            CreatedAt = DateTime.UtcNow.AddDays(-5),
            UpdatedAt = DateTime.UtcNow.AddDays(-3),
            UserFullName = "Ahmet Yılmaz",
            UserUsername = "cinemafan01"
        };
    }
}

public class CreateReviewRequestExample : IExamplesProvider<CreateReviewRequest>
{
    public CreateReviewRequest GetExamples()
    {
        return new CreateReviewRequest
        {
            MovieId = Guid.Parse("550e8400-e29b-41d4-a716-446655440000"),
            Title = "Muhteşem bir sinema deneyimi",
            Content = "Bu film beni gerçekten etkiledi. Hikayesi, oyuncuları ve görsel efektleri ile mükemmel bir yapım. Özellikle ana karakterin gelişimi çok iyi işlenmiş. Kesinlikle tavsiye ederim!",
            ContainsSpoilers = false
        };
    }
}

public class UpdateReviewRequestExample : IExamplesProvider<UpdateReviewRequest>
{
    public UpdateReviewRequest GetExamples()
    {
        return new UpdateReviewRequest
        {
            Title = "Güncellenmiş: Harika bir film!",
            Content = "İlk izlediğimde bu kadar etkileyici olduğunu düşünmemiştim. İkinci kez izledikten sonra detayları daha iyi fark ettim. Gerçekten başyapıt niteliğinde bir film.",
            ContainsSpoilers = false
        };
    }
}

public class CommentDtoExample : IExamplesProvider<CommentDto>
{
    public CommentDto GetExamples()
    {
        return new CommentDto
        {
            Id = Guid.Parse("550e8400-e29b-41d4-a716-446655440000"),
            ReviewId = Guid.Parse("660e8400-e29b-41d4-a716-446655440001"),
            UserId = Guid.Parse("770e8400-e29b-41d4-a716-446655440002"),
            Username = "blockbusterlover",
            Content = "Bence biraz fazla karmaşıktı ama yine de güzel bir filmdi. Görsel efektler harikasaydı!",
            UpvotesCount = 8,
            DownvotesCount = 1,
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            UpdatedAt = null,
            ParentCommentId = null,
            Replies = new List<CommentDto>
            {
                new CommentDto
                {
                    Id = Guid.NewGuid(),
                    ReviewId = Guid.Parse("660e8400-e29b-41d4-a716-446655440001"),
                    UserId = Guid.NewGuid(),
                    Username = "indiefilmfan",
                    Content = "Karmaşık olması filmin güzelliği bence!",
                    UpvotesCount = 3,
                    DownvotesCount = 0,
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    ParentCommentId = Guid.Parse("550e8400-e29b-41d4-a716-446655440000"),
                    Replies = new List<CommentDto>()
                }
            }
        };
    }
}

public class CreateCommentRequestExample : IExamplesProvider<CreateCommentRequest>
{
    public CreateCommentRequest GetExamples()
    {
        return new CreateCommentRequest
        {
            ReviewId = Guid.Parse("550e8400-e29b-41d4-a716-446655440000"),
            Content = "Bu inceleme çok başarılı! Ben de aynı şekilde düşünüyorum. Özellikle görsel efektler konusunda haklısınız.",
            ParentCommentId = null
        };
    }
}

public class UpdateCommentRequestExample : IExamplesProvider<UpdateCommentRequest>
{
    public UpdateCommentRequest GetExamples()
    {
        return new UpdateCommentRequest
        {
            Content = "Düzenlendi: Bu inceleme gerçekten çok başarılı! Özellikle karakterlerin gelişimi konusunda çok detaylı analiz yapmışsınız."
        };
    }
}

public class RatingDtoExample : IExamplesProvider<RatingDto>
{
    public RatingDto GetExamples()
    {
        return new RatingDto
        {
            Id = Guid.Parse("550e8400-e29b-41d4-a716-446655440000"),
            UserId = Guid.Parse("660e8400-e29b-41d4-a716-446655440001"),
            MovieId = Guid.Parse("770e8400-e29b-41d4-a716-446655440002"),
            Score = 9,
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            UpdatedAt = DateTime.UtcNow.AddDays(-5)
        };
    }
}

public class CreateRatingRequestExample : IExamplesProvider<CreateRatingRequest>
{
    public CreateRatingRequest GetExamples()
    {
        return new CreateRatingRequest
        {
            MovieId = Guid.Parse("550e8400-e29b-41d4-a716-446655440000"),
            Score = 8
        };
    }
}

public class ReactionDtoExample : IExamplesProvider<ReactionDto>
{
    public ReactionDto GetExamples()
    {
        return new ReactionDto
        {
            Id = Guid.Parse("550e8400-e29b-41d4-a716-446655440000"),
            UserId = Guid.Parse("660e8400-e29b-41d4-a716-446655440001"),
            CommentId = Guid.Parse("770e8400-e29b-41d4-a716-446655440002"),
            Type = ReactionType.Upvote,
            CreatedAt = DateTime.UtcNow.AddMinutes(-30),
            UpdatedAt = null
        };
    }
}

public class CreateReactionRequestExample : IExamplesProvider<CreateReactionRequest>
{
    public CreateReactionRequest GetExamples()
    {
        return new CreateReactionRequest
        {
            CommentId = Guid.Parse("550e8400-e29b-41d4-a716-446655440000"),
            Type = ReactionType.Upvote
        };
    }
}

public class FavoriteDtoExample : IExamplesProvider<FavoriteDto>
{
    public FavoriteDto GetExamples()
    {
        return new FavoriteDto
        {
            Id = Guid.Parse("550e8400-e29b-41d4-a716-446655440000"),
            UserId = Guid.Parse("660e8400-e29b-41d4-a716-446655440001"),
            MovieId = Guid.Parse("770e8400-e29b-41d4-a716-446655440002"),
            MovieTitle = "Inception",
            MoviePosterPath = "/posters/inception.jpg",
            CreatedAt = DateTime.UtcNow.AddDays(-7)
        };
    }
}

public class AddToFavoritesRequestExample : IExamplesProvider<AddToFavoritesRequest>
{
    public AddToFavoritesRequest GetExamples()
    {
        return new AddToFavoritesRequest
        {
            MovieId = Guid.Parse("550e8400-e29b-41d4-a716-446655440000")
        };
    }
}