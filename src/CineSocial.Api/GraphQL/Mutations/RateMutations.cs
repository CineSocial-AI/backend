using CineSocial.Application.UseCases.Rates;
using HotChocolate;
using HotChocolate.Authorization;

namespace CineSocial.Api.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class RateMutations
{
    [Authorize]
    public async Task<bool> RateMovie(
        int movieId,
        decimal rating,
        [Service] RateMovieUseCase useCase,
        CancellationToken cancellationToken)
    {
        return await useCase.ExecuteAsync(movieId, rating, cancellationToken);
    }

    [Authorize]
    public async Task<bool> RemoveRate(
        int movieId,
        [Service] RemoveRateUseCase useCase,
        CancellationToken cancellationToken)
    {
        return await useCase.ExecuteAsync(movieId, cancellationToken);
    }
}
