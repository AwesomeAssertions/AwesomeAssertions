using System;

namespace FluentAssertions.Formatting;

/// <summary>
/// Holds a <see cref="Type"/> which should be formatted in short notation, i.e. without any namespaces.
/// </summary>
/// <param name="type">The type to format.</param>
internal sealed class ShortTypeValue(Type type)
{
    /// <summary>
    /// The type to format.
    /// </summary>
    public Type Type { get; } = type;
}
