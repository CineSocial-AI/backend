using CineSocial.Core.Features.Comments.Commands;
using CineSocial.Core.Localization;
using FluentValidation;

namespace CineSocial.Core.Features.Comments.Validators;

public class UpdateCommentCommandValidator : AbstractValidator<UpdateCommentCommand>
{
    private readonly ILocalizationService _localizationService;

    public UpdateCommentCommandValidator(ILocalizationService localizationService)
    {
        _localizationService = localizationService;
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage(_localizationService.GetValidationString("Required.CommentId"));

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage(_localizationService.GetValidationString("Required.UserId"));

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage(_localizationService.GetValidationString("Required.Content"))
            .Length(2, 500).WithMessage(_localizationService.GetValidationString("Length.Content.Comment"));
    }
}