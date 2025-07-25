using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using AwesomeAssertions.Execution;
using AwesomeAssertions.Primitives;

namespace AwesomeAssertions;

/// <summary>
/// Contains an extension method for custom assertions in unit tests related to Enum objects.
/// </summary>
[DebuggerNonUserCode]
public static class EnumAssertionsExtensions
{
    /// <summary>
    /// Returns an <see cref="EnumAssertions{TEnum, TAssertions}"/> object that can be used to assert the
    /// current <typeparamref name="TEnum"/>.
    /// </summary>
    [Pure]
    public static EnumAssertions<TEnum> Should<TEnum>(this TEnum @enum)
        where TEnum : struct, Enum
    {
        return new EnumAssertions<TEnum>(@enum, AssertionChain.GetOrCreate());
    }

    /// <summary>
    /// Returns an <see cref="EnumAssertions{TEnum, TAssertions}"/> object that can be used to assert the
    /// current <typeparamref name="TEnum"/>.
    /// </summary>
    [Pure]
    public static NullableEnumAssertions<TEnum> Should<TEnum>(this TEnum? @enum)
        where TEnum : struct, Enum
    {
        return new NullableEnumAssertions<TEnum>(@enum, AssertionChain.GetOrCreate());
    }
}
