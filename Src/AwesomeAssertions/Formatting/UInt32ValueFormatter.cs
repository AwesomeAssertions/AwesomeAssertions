using System.Globalization;

namespace AwesomeAssertions.Formatting;

public class UInt32ValueFormatter : IValueFormatter
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
        return value is uint;
    }

    public void Format(object value, FormattedObjectGraph formattedGraph, FormattingContext context, FormatChild formatChild)
    {
        formattedGraph.AddFragment(((uint)value).ToString(CultureInfo.InvariantCulture) + "u");
    }
}
