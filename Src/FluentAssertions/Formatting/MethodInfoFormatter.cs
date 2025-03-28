using System;
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
            FormatOperator(method, "implicit", formattedGraph, formatChild);
        }
        else if (method.IsSpecialName && method.Name == "op_Explicit")
        {
            FormatOperator(method, "explicit", formattedGraph, formatChild);
        }
        else
        {
            formatChild("type", method!.DeclaringType.AsFormattableShortType(), formattedGraph);
            formattedGraph.AddFragment($".{method.Name}");
        }
    }

    private static void FormatOperator(MethodInfo method, string operatorType, FormattedObjectGraph formattedGraph, FormatChild formatChild)
    {
        formattedGraph.AddFragment($"{operatorType} operator ");
        formatChild("type", method.ReturnType.AsFormattableShortType(), formattedGraph);
        formattedGraph.AddFragment("(");
        formatChild("type", method.GetParameters()[0].ParameterType.AsFormattableShortType(), formattedGraph);
        formattedGraph.AddFragment(")");
    }
}
