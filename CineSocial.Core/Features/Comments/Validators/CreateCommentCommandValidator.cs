using CineSocial.Core.Features.Comments.Commands;
using CineSocial.Core.Localization;
using FluentValidation;

namespace CineSocial.Core.Features.Comments.Validators;

public class CreateCommentCommandValidator : AbstractValidator<CreateCommentCommand>
{
    private readonly ILocalizationService _localizationService;

    public CreateCommentCommandValidator(ILocalizationService localizationService)
    {
        _localizationService = localizationService;
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage(_localizationService.GetValidationString("Required.UserId"));

        RuleFor(x => x.ReviewId)
            .NotEmpty().WithMessage(_localizationService.GetValidationString("Required.ReviewId"));

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage(_localizationService.GetValidationString("Required.Content"))
            .Length(2, 500).WithMessage(_localizationService.GetValidationString("Length.Content.Comment"));
    }
}