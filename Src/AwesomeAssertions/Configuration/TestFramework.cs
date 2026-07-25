namespace AwesomeAssertions.Configuration;

/// <summary>
/// The test frameworks supported by Awesome Assertions.
/// </summary>
public enum TestFramework
{
    /// <summary>
    ///     xUnit.net version 2.
    /// </summary>
    XUnit2,

    /// <summary>
    ///     xUnit.net version 3.
    /// </summary>
    XUnit3,

    /// <summary>
    ///     TUnit.
    /// </summary>
    TUnit,

    /// <summary>
    ///     MSTest version 2 an 3.
    /// </summary>
    MsTest,

    /// <summary>
    ///     NUnit.
    /// </summary>
    NUnit,

    /// <summary>
    ///     Machine.Specifications (MSpec).
    /// </summary>
    MSpec,

    /// <summary>
    ///     MSTest version 4.
    /// </summary>
    MsTest4
}
