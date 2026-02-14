namespace AwesomeAssertions.Common;

/// <summary>
/// The first difference item between two collections.
/// </summary>
/// <typeparam name="TFirst">Type of first collection.</typeparam>
/// <typeparam name="TSecond">Type of second collection.</typeparam>
internal sealed class CollectionDifferenceItem<TFirst, TSecond>
{
    /// <summary>
    /// True if there is a difference between the two collections, false otherwise.
    /// </summary>
    public bool HasDifference => Index < 0;

    /// <summary>
    /// Index where the first difference occurs, or -1 if there is no difference.
    /// </summary>
    public required int Index { get; init; }

    /// <summary>
    /// Item from the first collection at the index of the first difference.
    /// </summary>
    public TFirst First { get; init; }

    /// <summary>
    /// Item from the second collection at the index of the first difference.
    /// </summary>
    public TSecond Second { get; init; }
}
