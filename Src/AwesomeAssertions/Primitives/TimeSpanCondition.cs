namespace AwesomeAssertions.Primitives;

/// <summary>
/// Defines how the actual time difference between two values is expected to relate to a specified time span.
/// </summary>
public enum TimeSpanCondition
{
    /// <summary>
    /// The actual time difference is expected to be greater than the specified time span.
    /// </summary>
    MoreThan,

    /// <summary>
    /// The actual time difference is expected to be greater than or equal to the specified time span.
    /// </summary>
    AtLeast,

    /// <summary>
    /// The actual time difference is expected to be exactly equal to the specified time span.
    /// </summary>
    Exactly,

    /// <summary>
    /// The actual time difference is expected to be less than or equal to the specified time span.
    /// </summary>
    Within,

    /// <summary>
    /// The actual time difference is expected to be less than the specified time span.
    /// </summary>
    LessThan
}
