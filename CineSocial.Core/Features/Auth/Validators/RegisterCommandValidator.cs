using CineSocial.Core.Features.Auth.Commands;
using CineSocial.Core.Localization;
using FluentValidation;

namespace CineSocial.Core.Features.Auth.Validators;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    private readonly ILocalizationService _localizationService;

    public RegisterCommandValidator(ILocalizationService localizationService)
    {
        _localizationService = localizationService;
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage(_localizationService.GetValidationString("Required.Username"))
            .Length(3, 50).WithMessage(_localizationService.GetValidationString("Length.Username"))
            .Matches("^[a-zA-Z0-9_]+$").WithMessage(_localizationService.GetValidationString("Format.Username"));

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(_localizationService.GetValidationString("Required.Email"))
            .EmailAddress().WithMessage(_localizationService.GetValidationString("Format.Email"));

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(_localizationService.GetValidationString("Required.Password"))
            .MinimumLength(6).WithMessage(_localizationService.GetValidationString("Length.Password"))
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)").WithMessage(_localizationService.GetValidationString("Format.Password"));

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage(_localizationService.GetValidationString("Required.FirstName"))
            .Length(2, 50).WithMessage(_localizationService.GetValidationString("Length.FirstName"));

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage(_localizationService.GetValidationString("Required.LastName"))
            .Length(2, 50).WithMessage(_localizationService.GetValidationString("Length.LastName"));

        RuleFor(x => x.Bio)
            .MaximumLength(500).WithMessage(_localizationService.GetValidationString("Length.Bio"))
            .When(x => !string.IsNullOrEmpty(x.Bio));
    }
}