#if NET
using System;
using System.Globalization;
using System.Numerics;

namespace AwesomeAssertions.Formatting;

/// <summary>
/// Formatter for <see cref="INumber{T}"/> types, which don't have their own formatter.
/// </summary>
internal sealed class NumericValueFormatter : IValueFormatter
{
    public bool CanHandle(object value) => Array.Exists(value.GetType().GetInterfaces(),
        ifc => ifc.IsGenericType && ifc.GetGenericTypeDefinition() == typeof(INumber<>));

    public void Format(object value, FormattedObjectGraph formattedGraph, FormattingContext context, FormatChild formatChild)
        => formattedGraph.AddFragment(((IFormattable)value).ToString(format: null, CultureInfo.InvariantCulture));
}
#endif
