namespace AwesomeAssertions.Equivalency;

/// <summary>
/// Represents the outcome of executing an <see cref="IEquivalencyStep"/> during a structural equivalency assertion.
/// </summary>
public enum EquivalencyResult
{
    /// <summary>
    /// The step did not handle the comparison, so the next <see cref="IEquivalencyStep"/> should be executed.
    /// </summary>
    ContinueWithNext,

    /// <summary>
    /// The step handled the comparison and proved the equivalency, so no further steps need to be executed.
    /// </summary>
    EquivalencyProven
}
