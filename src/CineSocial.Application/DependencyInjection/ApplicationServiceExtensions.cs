using CineSocial.Application.UseCases.Blocks;
using CineSocial.Application.UseCases.Comments;
using CineSocial.Application.UseCases.Follows;
using CineSocial.Application.UseCases.Movies;
using CineSocial.Application.UseCases.Rates;
using CineSocial.Application.UseCases.Reactions;
using CineSocial.Application.UseCases.Users;
using Microsoft.Extensions.DependencyInjection;

namespace CineSocial.Application.DependencyInjection;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(ApplicationServiceExtensions).Assembly));

        // User UseCases
        services.AddScoped<GetCurrentUserUseCase>();
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

        return services;
    }
}
