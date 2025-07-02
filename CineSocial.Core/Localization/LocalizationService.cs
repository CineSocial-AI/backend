using Microsoft.Extensions.Localization;
using System.Globalization;

namespace CineSocial.Core.Localization;

public class LocalizationService : ILocalizationService
{
    private readonly IStringLocalizer<LocalizationService> _localizer;
    private readonly IStringLocalizer<ValidationMessages> _validationLocalizer;

    public LocalizationService(
        IStringLocalizer<LocalizationService> localizer,
        IStringLocalizer<ValidationMessages> validationLocalizer)
    {
        _localizer = localizer;
        _validationLocalizer = validationLocalizer;
    }

    public string GetString(string key)
    {
        var localizedString = _localizer[key];
        return localizedString.ResourceNotFound ? key : localizedString.Value;
    }

    public string GetString(string key, params object[] arguments)
    {
        var localizedString = _localizer[key, arguments];
        return localizedString.ResourceNotFound ? key : localizedString.Value;
    }

    public string GetValidationString(string key)
    {
        var localizedString = _validationLocalizer[key];
        return localizedString.ResourceNotFound ? key : localizedString.Value;
    }

    public string GetValidationString(string key, params object[] arguments)
    {
        var localizedString = _validationLocalizer[key, arguments];
        return localizedString.ResourceNotFound ? key : localizedString.Value;
    }

    public void SetCulture(string cultureCode)
    {
        var culture = new CultureInfo(cultureCode);
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
    }

    public string GetCurrentCulture()
    {
        return CultureInfo.CurrentUICulture.Name;
    }
}

// Dummy classes for resource files
public class ValidationMessages { }