#if NET

using System;
using System.Globalization;

namespace AwesomeAssertions.Formatting;

/// <summary>
/// Provides a human-readable representation of <see cref="System.DateOnly"/> values.
/// </summary>
public class DateOnlyValueFormatter : IValueFormatter
{
    /// <summary>
    /// Indicates whether the current <see cref="IValueFormatter"/> can handle the specified <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The value for which to create a <see cref="string"/>.</param>
    /// <returns>
    /// <see langword="true"/> if the current <see cref="IValueFormatter"/> can handle the specified value; otherwise, <see langword="false"/>.
    /// </returns>
    public bool CanHandle(object value)
    {
        return value is DateOnly;
    }

    /// <inheritdoc />
    public void Format(object value, FormattedObjectGraph formattedGraph, FormattingContext context, FormatChild formatChild)
    {
        var dateOnly = (DateOnly)value;
        formattedGraph.AddFragment(dateOnly.ToString("<yyyy-MM-dd>", CultureInfo.InvariantCulture));
    }
}

#endif
