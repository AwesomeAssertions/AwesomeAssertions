using System;

namespace AwesomeAssertions.Common.Mismatch;

/// <summary>
/// Inverts the basis for the <see cref="StandardTruncationStrategy"/>.
/// </summary>
/// <remarks>
/// In relation to <see cref="StandardTruncationStrategy"/>, this would shift the window of visible text left in the output given the same input.
/// The target index would appear to shift right as a result of the window shifting left. The strategy would be biased toward omitting words from
/// the left as opposed to the right.
/// </remarks>
internal class InverseTruncationBasisDecorator(StandardTruncationStrategy truncationStrategy) : ITruncationStrategy
{
    public Range GetTruncationRange(string text, int targetIndex)
    {
        var reversedIndex = GetReverseIndex(targetIndex, text.Length);
        var initialRange = truncationStrategy.GetTruncationRange(text.Reversed(), reversedIndex);
        return GetReverseRange(text.Length, initialRange);
    }

    /// <summary>
    /// Converts a zero-based index into its equivalent position when reversing a collection of the specified length.
    /// </summary>
    /// <param name="index">The index to be reversed.</param>
    /// <param name="length">The total length of the collection.</param>
    /// <returns>
    /// The reversed index corresponding to the original index within the collection.
    /// </returns>
    private static int GetReverseIndex(int index, int length)
    {
        return length - index - 1;
    }

    /// <summary>
    /// Calculates the range of indices that correspond to reversing a subrange of elements within a collection of a specified length.
    /// </summary>
    /// <param name="length">The total length of the collection.</param>
    /// <param name="innerRange">The range to be reversed within the collection.</param>
    /// <returns>
    /// A range representing the reversed indices corresponding to the input range within the collection.
    /// </returns>
    private static Range GetReverseRange(int length, Range innerRange)
    {
        return new Range(GetReverseIndex(innerRange.End.Value - 1, length), GetReverseIndex(innerRange.Start.Value, length) + 1);
    }
}
