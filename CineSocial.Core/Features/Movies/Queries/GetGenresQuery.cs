using CineSocial.Core.Shared;
using CineSocial.Core.Shared.Interfaces;
using MediatR;

namespace CineSocial.Core.Features.Movies.Queries;

public record GetGenresQuery : IRequest<Result<List<GenreResult>>>;

public class GetGenresQueryHandler : IRequestHandler<GetGenresQuery, Result<List<GenreResult>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetGenresQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<GenreResult>>> Handle(GetGenresQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var genres = await _unitOfWork.Genres.GetAllAsync(cancellationToken);
            
            var result = genres.Select(g => new GenreResult(
                g.Id,
                g.Name,
                g.Description ?? ""
            )).OrderBy(g => g.Name).ToList();

            return Result<List<GenreResult>>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<List<GenreResult>>.Failure($"Türler yüklenirken hata oluştu: {ex.Message}");
        }
    }
}