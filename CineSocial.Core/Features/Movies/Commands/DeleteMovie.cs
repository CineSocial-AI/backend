using CineSocial.Core.Shared;
using CineSocial.Core.Shared.Interfaces;
using FluentValidation;
using MediatR;

namespace CineSocial.Core.Features.Movies.Commands;

public record DeleteMovieCommand(Guid Id) : IRequest<Result>;

public class DeleteMovieValidator : AbstractValidator<DeleteMovieCommand>
{
    public DeleteMovieValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Movie ID is required");
    }
}

public class DeleteMovieHandler : IRequestHandler<DeleteMovieCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteMovieHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteMovieCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var movie = await _unitOfWork.Movies.GetByIdAsync(request.Id, cancellationToken);

            if (movie == null)
            {
                return Result.Failure(ErrorTypes.NotFound.Movie);
            }

            _unitOfWork.Movies.Remove(movie);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ErrorTypes.System.DatabaseError);
        }
    }
}