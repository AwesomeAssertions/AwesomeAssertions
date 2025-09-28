using System;
using System.Linq;

namespace AwesomeAssertions.Common.Mismatch;

/// <summary>
/// Represents a span of text with a mismatch identified at a specific index.
/// </summary>
internal sealed class MismatchSpan
{
    /// <summary>
    /// Gets a value indicating whether the start has been truncated compared to the original text.
    /// </summary>
    public bool StartTruncated { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the end has been truncated compared to the original text.
    /// </summary>
    public bool EndTruncated { get; private set; }

    /// <summary>
    /// Gets the mismatch index relative to the start of the span.
    /// </summary>
    public int MismatchIndex { get; private set; }

    /// <summary>
    /// Gets the length of the span.
    /// </summary>
    public int Length => Text.Length;

    /// <summary>
    /// Gets the visible text.
    /// </summary>
    public string Text { get; private set; }

    public MismatchSpan(string text, int mismatchIndex)
    {
        Text = text;
        MismatchIndex = mismatchIndex;
    }

    /// <summary>
    /// Truncates the span around the mismatch index.
    /// </summary>
    public void Truncate(ITruncationStrategy truncationStrategy)
    {
        var range = truncationStrategy.GetTruncationRange(Text, MismatchIndex);
        Truncate(range);
    }

    /// <summary>
    /// Escapes new lines within the text span.
    /// </summary>
    public void EscapeNewLines()
    {
        var indexOffset = Text
            .Take(MismatchIndex)
            .Count(c => c is '\n' or '\r');

        MismatchIndex += indexOffset;

        Text = Text
            .Replace("\n", "\\n", StringComparison.OrdinalIgnoreCase)
            .Replace("\r", "\\r", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Prepends text to the span.
    /// </summary>
    /// <param name="value"></param>
    public void Prepend(string value)
    {
        Text = value + Text;
        MismatchIndex += value.Length;
    }

    /// <summary>
    /// Appends text to the span.
    /// </summary>
    /// <param name="value"></param>
    public void Append(string value)
    {
        Text += value;
    }

    private void Truncate(Range range)
    {
        if (StartsBefore(range))
        {
            StartTruncated = true;
            MismatchIndex -= range.Start.Value;
        }

        if (EndsAfter(range))
        {
            EndTruncated = true;
        }

        Text = Text[range];
    }

    /// <summary>
    /// Returns a value indicating whether the range starts before the span.
    /// </summary>
    private static bool StartsBefore(Range range)
    {
        return range.Start.Value > 0;
    }

    /// <summary>
    /// Returns a value indicating whether the range ends after the span.
    /// </summary>
    private bool EndsAfter(Range range)
    {
        return Length > range.End.Value;
    }
}
