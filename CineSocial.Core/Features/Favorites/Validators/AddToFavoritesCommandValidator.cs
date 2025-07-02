using CineSocial.Core.Features.Favorites.Commands;
using CineSocial.Core.Localization;
using FluentValidation;

namespace CineSocial.Core.Features.Favorites.Validators;

public class AddToFavoritesCommandValidator : AbstractValidator<AddToFavoritesCommand>
{
    private readonly ILocalizationService _localizationService;

    public AddToFavoritesCommandValidator(ILocalizationService localizationService)
    {
        _localizationService = localizationService;
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage(_localizationService.GetValidationString("Required.UserId"));

        RuleFor(x => x.MovieId)
            .NotEmpty().WithMessage(_localizationService.GetValidationString("Required.MovieId"));
    }
}