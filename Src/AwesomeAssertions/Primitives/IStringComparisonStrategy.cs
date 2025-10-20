using AwesomeAssertions.Execution;

namespace AwesomeAssertions.Primitives;

/// <summary>
/// The strategy used for comparing two <see langword="string" />s.
/// </summary>
internal interface IStringComparisonStrategy
{
    /// <summary>
    /// Asserts that the <paramref name="subject"/> matches the <paramref name="expected"/> value.
    /// </summary>
    void ValidateAgainstMismatch(AssertionChain assertionChain, string subject, string expected);
}
