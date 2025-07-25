using System;

#pragma warning disable CA1714
namespace AwesomeAssertions.Equivalency;

/// <summary>
/// Determines which members are included in the equivalency assertion
/// </summary>
[Flags]
public enum MemberVisibility
{
    None = 0,
    Internal = 1,
    Public = 2,
    ExplicitlyImplemented = 4,
    DefaultInterfaceProperties = 8
}
