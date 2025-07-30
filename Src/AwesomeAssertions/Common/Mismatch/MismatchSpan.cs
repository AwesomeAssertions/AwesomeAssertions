using System;
using System.Text;

namespace AwesomeAssertions.Common.Mismatch;

internal sealed class MismatchSpan
{
    private readonly StringBuilder prependBuffer = new StringBuilder();

    private readonly StringBuilder appendBuffer = new StringBuilder();

    private readonly StringBuilder textBuffer = new StringBuilder();

    private string baseText;

    private int start;

    private int end;

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

    private MismatchSpan(string text, int mismatchIndex)
    {
        baseText = text;
        MismatchIndex = mismatchIndex;
        start = 0;
        end = text.Length;
    }

    public static MismatchSpan Create(string text, int mismatchIndex)
    {
        return new MismatchSpan(text, mismatchIndex);
    }

    private int InnerLength => end - start;

    /// <summary>
    /// The starting index of the span in reference to a parent span.
    /// </summary>
    public int Start => start + prependBuffer.Length;

    /// <summary>
    /// The (exclusive) ending index of the span in reference to a parent span.
    /// </summary>
    public int End => prependBuffer.Length + end;

    /// <summary>
    /// The length of the span.
    /// </summary>
    public int Length => prependBuffer.Length + InnerLength + appendBuffer.Length;

    /// <summary>
    /// The visible range of text within the span.
    /// </summary>
    public string Text
    {
        get
        {
            textBuffer.Clear();
            textBuffer.EnsureCapacity(Length);
            return textBuffer
                .Append(prependBuffer)
                .Append(baseText, start, InnerLength)
                .Append(appendBuffer)
                .ToString();
        }
    }

    /// <summary>
    /// Truncates the span around the mismatch index.
    /// </summary>
    public void Truncate()
    {
        var range = TruncationAlgorithm.GetTruncationRange(Text, MismatchIndex);
        Truncate(range);
    }

    /// <summary>
    /// Truncates the span with a bias toward preserving more words on the left side of the mismatching index as opposed to the right.
    /// </summary>
    /// <remarks>
    /// In relation to <see cref="Truncate()"/>, this would shift the window of visible text leftward in the output given the same input.
    /// </remarks>
    public void TruncateReverse()
    {
        var reversedIndex = GetReverseIndex(MismatchIndex);
        var initialRange = TruncationAlgorithm.GetTruncationRange(Text.Reversed(), reversedIndex);
        var reversedRange = GetReverseRange(initialRange);
        Truncate(reversedRange);
    }

    /// <summary>
    /// Escapes new lines within the text span.
    /// </summary>
    public void EscapeNewLines()
    {
        var startOffset = 0;
        var mismatchOffset = 0;
        var endOffset = 0;
        var offsetMismatchIndex = start + MismatchIndex;
        var innerEndIndex = end - 1;

        var text = baseText;

        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] is not ('\r' or '\n'))
            {
                continue;
            }

            if (i < start)
            {
                startOffset++;
            }

            if (i >= start && i < offsetMismatchIndex)
            {
                mismatchOffset++;
            }

            if (i < innerEndIndex)
            {
                endOffset++;
            }
        }

        start += startOffset;
        end += endOffset;
        MismatchIndex += mismatchOffset;

        textBuffer.Clear();
        textBuffer.EnsureCapacity(baseText.Length + startOffset + endOffset);
        baseText = textBuffer
            .Append(baseText)
            .Replace("\r", "\\r")
            .Replace("\n", "\\n")
            .ToString();
    }

    /// <summary>
    /// Prepends text to the span.
    /// </summary>
    /// <param name="value"></param>
    public void Prepend(string value)
    {
        prependBuffer.Insert(0, value);
        MismatchIndex += value.Length;
    }

    /// <summary>
    /// Appends text to the span.
    /// </summary>
    /// <param name="value"></param>
    public void Append(string value)
    {
        appendBuffer.Append(value);
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
            start = range.Start.Value;
        }

        if (EndsAfter(range))
        {
            EndTruncated = true;
            end = range.End.Value;
        }
    }

    private bool StartsBefore(Range range)
    {
        return start < range.Start.Value;
    }

    private bool EndsAfter(Range range)
    {
        return end > range.End.Value;
    }

    private bool Contains(Range range)
    {
        return range.Start.Value >= start && range.End.Value <= end;
    }

    /// <summary>
    /// Mirrors the given index relative to the span. The returned index represents the new position of the item at that index if the span was reversed.
    /// </summary>
    /// <example>
    /// Given a span of [0, 1, 2*, 3, 4, 5, 6, 8, 9], the mirrored index of 2* would be 7, as is the case in the reversed span [9, 8, 7, 6, 5, 4, 3, 2*, 1, 0].
    /// </example>
    private int GetReverseIndex(int index)
    {
        return Length - index - 1;
    }

    /// <summary>
    /// Mirrors the indices of the range relative to the span. The returned range represents the range of the items if the span was reversed.
    /// </summary>
    /// <example>
    /// Given a span of [0, [1, 2, 3], 4, 5, 6, 8, 9], the new range of subspan in range 1..4 would be 6..9 in [9, 8, 7, 6, 5, 4, [3, 2, 1], 0].
    /// </example>
    private Range GetReverseRange(Range childSpan)
    {
        return new Range(GetReverseIndex(childSpan.End.Value - 1), GetReverseIndex(childSpan.Start.Value) + 1);
    }
}
