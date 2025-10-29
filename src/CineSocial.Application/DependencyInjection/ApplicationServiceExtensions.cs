using CineSocial.Application.Common.Behaviors;
using CineSocial.Application.UseCases.Blocks;
using CineSocial.Application.UseCases.Comments;
using CineSocial.Application.UseCases.Follows;
using CineSocial.Application.UseCases.MovieLists;
using CineSocial.Application.UseCases.Movies;
using CineSocial.Application.UseCases.Rates;
using CineSocial.Application.UseCases.Reactions;
using CineSocial.Application.UseCases.Users;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CineSocial.Application.DependencyInjection;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register MediatR
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(ApplicationServiceExtensions).Assembly));

        // Register MediatR Pipeline Behaviors (Order matters: Performance -> Logging -> Others)
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        // User UseCases
        services.AddScoped<GetCurrentUserUseCase>();
        services.AddScoped<GetAllUsersUseCase>();
        services.AddScoped<UpdateProfileUseCase>();

        // Movies UseCases
        services.AddScoped<GetMoviesUseCase>();
        services.AddScoped<GetMovieByIdUseCase>();

        // Comments UseCases
        services.AddScoped<GetMovieCommentsUseCase>();
        services.AddScoped<GetCommentRepliesUseCase>();
        services.AddScoped<CreateCommentUseCase>();
        services.AddScoped<ReplyToCommentUseCase>();
        services.AddScoped<UpdateCommentUseCase>();
        services.AddScoped<DeleteCommentUseCase>();

        // Rates UseCases
        services.AddScoped<GetMovieRatingStatsUseCase>();
        services.AddScoped<GetUserRateForMovieUseCase>();
        services.AddScoped<RateMovieUseCase>();
        services.AddScoped<RemoveRateUseCase>();

        // Reactions UseCases
        services.AddScoped<AddReactionUseCase>();
        services.AddScoped<RemoveReactionUseCase>();

        // Follow UseCases
        services.AddScoped<FollowUserUseCase>();
        services.AddScoped<UnfollowUserUseCase>();
        services.AddScoped<GetFollowersUseCase>();
        services.AddScoped<GetFollowingUseCase>();

        // Block UseCases
        services.AddScoped<BlockUserUseCase>();
        services.AddScoped<UnblockUserUseCase>();
        services.AddScoped<GetBlockedUsersUseCase>();

        // MovieList UseCases
        services.AddScoped<CreateMovieListUseCase>();
        services.AddScoped<UpdateMovieListUseCase>();
        services.AddScoped<DeleteMovieListUseCase>();
        services.AddScoped<GetMovieListByIdUseCase>();
        services.AddScoped<GetUserMovieListsUseCase>();
        services.AddScoped<GetPublicMovieListsUseCase>();
        services.AddScoped<GetUserWatchlistUseCase>();
        services.AddScoped<AddMovieToListUseCase>();
        services.AddScoped<RemoveMovieFromListUseCase>();
        services.AddScoped<ReorderMovieInListUseCase>();
        services.AddScoped<FavoriteMovieListUseCase>();
        services.AddScoped<UnfavoriteMovieListUseCase>();
        services.AddScoped<GetUserFavoriteListsUseCase>();

        return services;
    }
}
