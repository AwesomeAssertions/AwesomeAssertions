using System.Reflection;
using FluentAssertions.Common;

namespace FluentAssertions.Formatting;

public class MethodInfoFormatter : IValueFormatter
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
        return value is MethodInfo;
    }

    public void Format(object value, FormattedObjectGraph formattedGraph, FormattingContext context, FormatChild formatChild)
    {
        var method = (MethodInfo)value;
        if (method.IsSpecialName && method.Name == "op_Implicit")
        {
            formattedGraph.AddFragment(
                $"implicit operator {TypeValueFormatter.Format(method.ReturnType)}({TypeValueFormatter.Format(method.GetParameters()[0].ParameterType)})");
        }
        else if (method.IsSpecialName && method.Name == "op_Explicit")
        {
            formattedGraph.AddFragment(
                $"explicit operator {TypeValueFormatter.Format(method.ReturnType)}({TypeValueFormatter.Format(method.GetParameters()[0].ParameterType)})");
        }
        else
        {
            formattedGraph.AddFragment($"{TypeValueFormatter.Format(method!.DeclaringType!)}.{method.Name}");
        }
    }
}
