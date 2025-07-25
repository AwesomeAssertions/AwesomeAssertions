using System.Reflection;

namespace AwesomeAssertions.Formatting;

public class PropertyInfoFormatter : IValueFormatter
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
        return value is PropertyInfo;
    }

    public void Format(object value, FormattedObjectGraph formattedGraph, FormattingContext context, FormatChild formatChild)
    {
        var property = (PropertyInfo)value;
        formatChild("type", property.DeclaringType!.AsFormattableShortType(), formattedGraph);
        formattedGraph.AddFragment($".{property.Name}");
    }
}
