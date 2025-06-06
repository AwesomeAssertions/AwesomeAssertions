#if NET6_0_OR_GREATER

using System;
using System.Globalization;

namespace AwesomeAssertions.Formatting;

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

    public void Format(object value, FormattedObjectGraph formattedGraph, FormattingContext context, FormatChild formatChild)
    {
        var dateOnly = (DateOnly)value;
        formattedGraph.AddFragment(dateOnly.ToString("<yyyy-MM-dd>", CultureInfo.InvariantCulture));
    }
}

#endif
