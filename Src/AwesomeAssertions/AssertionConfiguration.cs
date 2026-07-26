using AwesomeAssertions.Configuration;

namespace AwesomeAssertions;

/// <summary>
/// Provides access to the global configuration and options to customize the behavior of AwesomeAssertions.
/// </summary>
public static class AssertionConfiguration
{
    /// <summary>
    /// Gets the current <see cref="GlobalConfiguration"/> used to customize the behavior of AwesomeAssertions.
    /// </summary>
    public static GlobalConfiguration Current => AssertionEngine.Configuration;
}
