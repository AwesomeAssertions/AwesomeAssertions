namespace AwesomeAssertions.Configuration;

/// <summary>
/// The test frameworks supported by Awesome Assertions.
/// </summary>
public enum TestFramework
{
    XUnit2,
    XUnit3,
    TUnit,

    /// <summary>
    ///     MSTest version 2 an 3.
    /// </summary>
    MsTest,
    NUnit,
    MSpec,

    /// <summary>
    ///     MSTest version 4.
    /// </summary>
    MsTest4
}
