using System;

namespace FluentAssertions.Formatting;

internal static class TypeFormattingExtensions
{
    /// <summary>
    /// Gets a type which can be formatted to a type definition like Dictionary&lt;TKey, TValue&gt;
    /// </summary>
    /// <param name="type">The original type</param>
    /// <returns>The type's type definition, it generic, or the original type.</returns>
    public static Type ToFormattableTypeDefinition(this Type type) =>
        type.IsGenericType && !type.IsGenericTypeDefinition ? type.GetGenericTypeDefinition() : type;
}
