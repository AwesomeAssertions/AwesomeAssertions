using System;

namespace AwesomeAssertions.Common.Mismatch;

/// <summary>
/// Defines a strategy for truncating text with a focus on retaining a specific range around a target index.
/// </summary>
internal interface ITruncationStrategy
{
    /// <summary>
    /// Determines the range of text to retain around a <paramref name="targetIndex"/> of interest within the <paramref name="text"/>.
    /// </summary>
    /// <remarks>
    /// Truncation prioritizes the retention of text closer to the <paramref name="targetIndex"/>. It will dynamically adjust the
    /// truncation range in a best-effort attempt to preserve whole words between word boundaries.
    /// </remarks>
    /// <returns>
    /// The range of text to retain.
    /// </returns>
    Range GetTruncationRange(string text, int targetIndex);
}
