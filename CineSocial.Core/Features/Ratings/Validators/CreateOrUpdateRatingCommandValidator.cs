using CineSocial.Core.Features.Ratings.Commands;
using CineSocial.Core.Localization;
using FluentValidation;

namespace CineSocial.Core.Features.Ratings.Validators;

public class CreateOrUpdateRatingCommandValidator : AbstractValidator<CreateOrUpdateRatingCommand>
{
    private readonly ILocalizationService _localizationService;

    public CreateOrUpdateRatingCommandValidator(ILocalizationService localizationService)
    {
        _localizationService = localizationService;
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage(_localizationService.GetValidationString("Required.UserId"));

        RuleFor(x => x.MovieId)
            .NotEmpty().WithMessage(_localizationService.GetValidationString("Required.MovieId"));

        RuleFor(x => x.Score)
            .InclusiveBetween(1, 10).WithMessage(_localizationService.GetValidationString("Range.Rating"));
    }
}