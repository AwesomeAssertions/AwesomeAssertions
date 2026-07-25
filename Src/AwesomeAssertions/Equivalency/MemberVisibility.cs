using System;

#pragma warning disable CA1714
namespace AwesomeAssertions.Equivalency;

/// <summary>
/// Determines which members are included in the equivalency assertion
/// </summary>
[Flags]
public enum MemberVisibility
{
    /// <summary>
    /// No members are included.
    /// </summary>
    None = 0,

    /// <summary>
    /// Members with <c>internal</c> visibility are included.
    /// </summary>
    Internal = 1,

    /// <summary>
    /// Members with <c>public</c> visibility are included.
    /// </summary>
    Public = 2,

    /// <summary>
    /// Explicitly implemented interface members are included.
    /// </summary>
    ExplicitlyImplemented = 4,

    /// <summary>
    /// Default interface properties are included.
    /// </summary>
    DefaultInterfaceProperties = 8
}
