namespace AwesomeAssertions.Equivalency;

/// <summary>
/// Determines how strictly the ordering of a collection is enforced during a structural equivalency assertion.
/// </summary>
public enum OrderStrictness
{
    /// <summary>
    /// The elements must appear in the same order.
    /// </summary>
    Strict,

    /// <summary>
    /// The elements may appear in any order.
    /// </summary>
    NotStrict,

    /// <summary>
    /// The ordering is not relevant and no ordering preference has been specified.
    /// </summary>
    Irrelevant
}
