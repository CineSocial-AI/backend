using System.Text.RegularExpressions;

namespace CineSocial.Application.Common.Logging;

/// <summary>
/// Utility class for masking sensitive data in logs
/// </summary>
public static class SensitiveDataMasker
{
    private static readonly string[] SensitiveFieldNames = new[]
    {
        "password", "passwordhash", "token", "refreshtoken",
        "secret", "apikey", "authorization", "creditcard"
    };

    /// <summary>
    /// Masks email address (e.g., us***@domain.com)
    /// </summary>
    public static string MaskEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            return "***";

        var parts = email.Split('@');
        if (parts[0].Length <= 2)
            return $"**@{parts[1]}";

        return $"{parts[0].Substring(0, 2)}***@{parts[1]}";
    }

    /// <summary>
    /// Masks credit card number (shows only last 4 digits)
    /// </summary>
    public static string MaskCreditCard(string creditCard)
    {
        if (string.IsNullOrWhiteSpace(creditCard) || creditCard.Length < 4)
            return "****";

        var digitsOnly = Regex.Replace(creditCard, @"\D", "");
        if (digitsOnly.Length < 4)
            return "****";

        var lastFour = digitsOnly.Substring(digitsOnly.Length - 4);
        return $"****-****-****-{lastFour}";
    }

    /// <summary>
    /// Masks phone number (shows only last 4 digits)
    /// </summary>
    public static string MaskPhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber) || phoneNumber.Length < 4)
            return "****";

        var digitsOnly = Regex.Replace(phoneNumber, @"\D", "");
        if (digitsOnly.Length < 4)
            return "****";

        var lastFour = digitsOnly.Substring(digitsOnly.Length - 4);
        return $"***-***-{lastFour}";
    }

    /// <summary>
    /// Masks a token or secret (shows only first 4 chars)
    /// </summary>
    public static string MaskToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return "****";

        if (token.Length <= 8)
            return "****";

        return $"{token.Substring(0, 4)}...{token.Substring(token.Length - 4)}";
    }

    /// <summary>
    /// Completely masks a sensitive value
    /// </summary>
    public static string MaskCompletely(string value)
    {
        return string.IsNullOrWhiteSpace(value) ? "****" : "********";
    }

    /// <summary>
    /// Checks if a field name is sensitive
    /// </summary>
    public static bool IsSensitiveField(string fieldName)
    {
        if (string.IsNullOrWhiteSpace(fieldName))
            return false;

        var lowerFieldName = fieldName.ToLowerInvariant();
        return SensitiveFieldNames.Any(sensitive => lowerFieldName.Contains(sensitive));
    }

    /// <summary>
    /// Masks an object's sensitive properties (for logging)
    /// Returns a dictionary with masked values
    /// </summary>
    public static Dictionary<string, object?> MaskObject(object obj)
    {
        var result = new Dictionary<string, object?>();

        if (obj == null)
            return result;

        var properties = obj.GetType().GetProperties();

        foreach (var prop in properties)
        {
            try
            {
                var propName = prop.Name;
                var propValue = prop.GetValue(obj);

                if (propValue == null)
                {
                    result[propName] = null;
                    continue;
                }

                // Check if this is a sensitive field
                if (IsSensitiveField(propName))
                {
                    result[propName] = MaskCompletely(propValue.ToString() ?? "");
                }
                else if (propName.ToLowerInvariant().Contains("email"))
                {
                    result[propName] = MaskEmail(propValue.ToString() ?? "");
                }
                else
                {
                    result[propName] = propValue;
                }
            }
            catch
            {
                // Skip properties that can't be read
                result[prop.Name] = "[unreadable]";
            }
        }

        return result;
    }
}
