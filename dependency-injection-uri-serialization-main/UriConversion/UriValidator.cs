using System;
using Validation;

namespace UriConversion;

/// <summary>
/// Uri string validator.
/// </summary>
public class UriValidator : IValidator<string>
{
    public UriValidator()
    {
    }

    public bool IsValid(string? obj)
    {
        ArgumentNullException.ThrowIfNull(obj);

        if (string.IsNullOrWhiteSpace(obj))
        {
            return false;
        }

        // Только абсолютные URI
        if (!Uri.TryCreate(obj, UriKind.Absolute, out var uri))
        {
            return false;
        }

        // Обязательные части
        if (string.IsNullOrWhiteSpace(uri.Scheme) ||
            string.IsNullOrWhiteSpace(uri.Host))
        {
            return false;
        }

        // Отсекаем мусор вроде "https:.php"
        if (!obj.Contains("://", StringComparison.Ordinal))
        {
            return false;
        }

        return true;
    }
}
