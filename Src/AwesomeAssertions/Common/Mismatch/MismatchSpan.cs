using System;
using System.Linq;

namespace AwesomeAssertions.Common.Mismatch;

internal sealed class MismatchSpan
{
    /// <summary>
    /// Whether the start has been truncated compared to the original text.
    /// </summary>
    public bool StartTruncated { get; private set; }

    /// <summary>
    /// Whether the end has been truncated compared to the original text.
    /// </summary>
    public bool EndTruncated { get; private set; }

    /// <summary>
    /// The mismatch index relative to the start of the span.
    /// </summary>
    public int MismatchIndex { get; private set; }

    /// <summary>
    /// The length of the span.
    /// </summary>
    public int Length => Text.Length;

    /// <summary>
    /// The visible text.
    /// </summary>
    public string Text { get; private set; }

    private MismatchSpan(string text, int mismatchIndex)
    {
        Text = text;
        MismatchIndex = mismatchIndex;
    }

    public static MismatchSpan Create(string text, int mismatchIndex)
    {
        return new MismatchSpan(text, mismatchIndex);
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
        if (!Contains(range))
        {
            return;
        }

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

    private static bool StartsBefore(Range range)
    {
        return range.Start.Value > 0;
    }

    private bool EndsAfter(Range range)
    {
        return Length > range.End.Value;
    }

    private bool Contains(Range range)
    {
        return range.Start.Value >= 0 && range.End.Value <= Length;
    }
}
