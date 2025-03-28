using System;

namespace FluentAssertions.Formatting;

internal static class TypeFormattingExtensions
{
    /// <summary>
    /// Gets a type which can be formatted to a type definition like Dictionary&lt;TKey, TValue&gt;
    /// </summary>
    /// <param name="type">The original type</param>
    /// <returns>The type's definition if generic, or the original type.</returns>
    public static Type AsFormattableTypeDefinition(this Type type)
    {
        if (type is not null && type.IsGenericType && !type.IsGenericTypeDefinition)
        {
            return type.GetGenericTypeDefinition();
        }

        return type;
    }

    /// <summary>
    /// Gets a type which can be formatted to a type definition like Dictionary&lt;TKey, TValue&gt; without namespaces.
    /// </summary>
    /// <param name="type">The original type</param>
    /// <returns>A value representing the type's definition if generic, or the original type for formatting.</returns>
    public static object AsFormattableShortTypeDefinition(this Type type)
    {
        if (type is null)
        {
            return null;
        }

        return new ShortTypeValue(type.AsFormattableTypeDefinition());
    }

    /// <summary>
    /// Gets a wrapper for formatting a type without namespaces.
    /// </summary>
    /// <param name="type">The original type</param>
    /// <returns>A value representing the incoming type for formatting without namespaces.</returns>
    public static ShortTypeValue AsFormattableShortType(this Type type) =>
        type is null ? null : new ShortTypeValue(type);
}
