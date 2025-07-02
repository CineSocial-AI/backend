using CineSocial.Core.Features.Auth.Commands;
using CineSocial.Core.Localization;
using FluentValidation;

namespace CineSocial.Core.Features.Auth.Validators;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    private readonly ILocalizationService _localizationService;

    public LoginCommandValidator(ILocalizationService localizationService)
    {
        _localizationService = localizationService;

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(_localizationService.GetValidationString("Required.Email"))
            .EmailAddress().WithMessage(_localizationService.GetValidationString("Format.Email"));

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(_localizationService.GetValidationString("Required.Password"))
            .MinimumLength(6).WithMessage(_localizationService.GetValidationString("Length.Password"));
    }
}