using AwesomeAssertions.Configuration;

namespace AwesomeAssertions;

/// <summary>
/// Provides access to the global configuration and options to customize the behavior of AwesomeAssertions.
/// </summary>
public static class AssertionConfiguration
{
    public static GlobalConfiguration Current => AssertionEngine.Configuration;
}
