using CineSocial.Core.Features.Reviews.Commands;
using CineSocial.Core.Localization;
using FluentValidation;

namespace CineSocial.Core.Features.Reviews.Validators;

public class UpdateReviewCommandValidator : AbstractValidator<UpdateReviewCommand>
{
    private readonly ILocalizationService _localizationService;

    public UpdateReviewCommandValidator(ILocalizationService localizationService)
    {
        _localizationService = localizationService;
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage(_localizationService.GetValidationString("Required.ReviewId"));

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage(_localizationService.GetValidationString("Required.UserId"));

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage(_localizationService.GetValidationString("Required.Title"))
            .MaximumLength(200).WithMessage(_localizationService.GetValidationString("Length.Title"));

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage(_localizationService.GetValidationString("Required.Content"))
            .Length(10, 2000).WithMessage(_localizationService.GetValidationString("Length.Content.Review"));
    }
}