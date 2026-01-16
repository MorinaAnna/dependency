using System;
using Conversion;
using LogerExtensionDelegate;
using Validation;

namespace UriConversion;

/// <summary>
/// The convertor class from string to Uri.
/// </summary>
public class UriConverter : IConverter<Uri?>
{
    private readonly IValidator<string> validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="UriConverter"/> class.
    /// </summary>
    /// <param name="validator">The string validator.</param>
    /// <exception cref="ArgumentNullException">Throw if validator is null.</exception>
    public UriConverter(IValidator<string>? validator)
    {
        this.validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    /// <summary>
    /// Converts the source string to Uri object.
    /// </summary>
    /// <param name="obj">The source string.</param>
    /// <returns> The Uri object if source string is valid and null otherwise.</returns>
    /// <exception cref="ArgumentNullException">Throw if source string is null.</exception>
    public Uri? Convert(string? obj)
    {
        ArgumentNullException.ThrowIfNull(obj);

        if (!this.validator.IsValid(obj))
        {
            return null;
        }

        return Uri.TryCreate(obj, UriKind.Absolute, out var uri) ? uri : null;
    }
}
