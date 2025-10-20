using AwesomeAssertions.Execution;

namespace AwesomeAssertions.Primitives;

/// <summary>
/// Base strategy for string comparisons which have the same kind of failure messages and null handling.
/// </summary>
internal abstract class StringComparisonBaseStrategy
{
    /// <summary>
    /// Expectation part of failure messages. Must not contain any formatting placeholder.
    /// </summary>
    protected abstract string ExpectationDescription { get; }

    /// <summary>
    /// Validates <paramref name="subject"/> and <paramref name="expected"/> against null values.
    /// </summary>
    /// <param name="assertionChain">Current assertion chain.</param>
    /// <param name="subject">Subject of the string assertion</param>
    /// <param name="expected">Expectation of the string assertion</param>
    /// <returns>True if following assertions should be executed, false otherwise</returns>
    protected bool ValidateAgainstNulls(AssertionChain assertionChain, string subject, string expected)
    {
        if (subject is null && expected is null)
        {
            // The case of both being null must be handled by the caller before calling the strategy.
            return false;
        }

        assertionChain
            .ForCondition(subject is not null && expected is not null)
            .FailWith($"{ExpectationDescription}{{0}}{{reason}}, but found {{1}}.", expected, subject);

        return assertionChain.Succeeded;
    }
}
