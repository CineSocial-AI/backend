using CineSocial.Core.Features.Reactions.Commands;
using CineSocial.Core.Localization;
using CineSocial.Domain.Enums;
using FluentValidation;

namespace CineSocial.Core.Features.Reactions.Validators;

public class CreateOrUpdateReactionCommandValidator : AbstractValidator<CreateOrUpdateReactionCommand>
{
    private readonly ILocalizationService _localizationService;

    public CreateOrUpdateReactionCommandValidator(ILocalizationService localizationService)
    {
        _localizationService = localizationService;
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage(_localizationService.GetValidationString("Required.UserId"));

        RuleFor(x => x.CommentId)
            .NotEmpty().WithMessage(_localizationService.GetValidationString("Required.CommentId"));

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage(_localizationService.GetValidationString("Valid.ReactionType"))
            .Must(type => type == ReactionType.Upvote || type == ReactionType.Downvote)
            .WithMessage(_localizationService.GetValidationString("Valid.ReactionType.UpvoteDownvote"));
    }
}