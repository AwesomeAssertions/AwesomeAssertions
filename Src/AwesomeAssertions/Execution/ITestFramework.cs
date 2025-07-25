using System.Diagnostics.CodeAnalysis;

namespace AwesomeAssertions.Execution;

/// <summary>
/// Represents an abstraction of a particular test framework such as MSTest, nUnit, etc.
/// </summary>
public interface ITestFramework
{
    /// <summary>
    /// Gets a value indicating whether the corresponding test framework is currently available.
    /// </summary>
    bool IsAvailable { get; }

    /// <summary>
    /// Throws a framework-specific exception to indicate a failing unit test.
    /// </summary>
    [DoesNotReturn]
    void Throw(string message);
}
