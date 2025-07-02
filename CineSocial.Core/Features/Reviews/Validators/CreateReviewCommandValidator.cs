using CineSocial.Core.Features.Reviews.Commands;
using CineSocial.Core.Localization;
using FluentValidation;

namespace CineSocial.Core.Features.Reviews.Validators;

public class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
{
    private readonly ILocalizationService _localizationService;

    public CreateReviewCommandValidator(ILocalizationService localizationService)
    {
        _localizationService = localizationService;
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage(_localizationService.GetValidationString("Required.UserId"));

        RuleFor(x => x.MovieId)
            .NotEmpty().WithMessage(_localizationService.GetValidationString("Required.MovieId"));

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage(_localizationService.GetValidationString("Required.Title"))
            .MaximumLength(200).WithMessage(_localizationService.GetValidationString("Length.Title"));

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage(_localizationService.GetValidationString("Required.Content"))
            .Length(10, 2000).WithMessage(_localizationService.GetValidationString("Length.Content.Review"));
    }
}