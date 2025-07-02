namespace CineSocial.Core.Localization;

public interface ILocalizationService
{
    string GetString(string key);
    string GetString(string key, params object[] arguments);
    string GetValidationString(string key);
    string GetValidationString(string key, params object[] arguments);
    void SetCulture(string cultureCode);
    string GetCurrentCulture();
}