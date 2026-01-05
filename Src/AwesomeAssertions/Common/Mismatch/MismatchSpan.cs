using System;
using System.Linq;

namespace AwesomeAssertions.Common.Mismatch;

/// <summary>
/// Represents a span of text with a mismatch identified at a specific index.
/// </summary>
internal sealed class MismatchSpan
{
    private readonly int stringPrintLength;

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

    public MismatchSpan(string text, int mismatchIndex, int stringPrintLength)
    {
        Text = text;
        MismatchIndex = mismatchIndex;
        this.stringPrintLength = stringPrintLength;
    }

    /// <summary>
    /// Truncates the span around the mismatch index.
    /// </summary>
    public void Truncate()
    {
        var range = GetTruncationRange(Text, MismatchIndex);
        Truncate(range);
    }

    /// <summary>
    /// Escapes white spaces within the text span.
    /// </summary>
    public void EscapeWhiteSpaces()
    {
        var indexOffset = Text
            .Take(MismatchIndex)
            .Count(c => c is '\n' or '\r' or '\t');

        MismatchIndex += indexOffset;

        Text = Text
            .Replace("\n", "\\n", StringComparison.Ordinal)
            .Replace("\r", "\\r", StringComparison.Ordinal)
            .Replace("\t", "\\t", StringComparison.Ordinal);
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

        if (EndsAfter(Length, range))
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
    private static bool EndsAfter(int length, Range range)
    {
        return length > range.End.Value;
    }

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
    private Range GetTruncationRange(string text, int targetIndex)
    {
        var start = GetStartIndexOfPhraseToShowBeforeTheTargetIndex(text, targetIndex);
        var length = GetLengthOfPhraseToShowOrDefaultLength(text[start..]);
        return new Range(start, start + length);
    }

    /// <summary>
    /// Calculates the start index of the visible segment from <paramref name="value"/> when highlighting the difference at <paramref name="targetIndex"/>.
    /// </summary>
    /// <remarks>
    /// Either keep the last 10 characters before <paramref name="targetIndex"/> or a word begin (separated by whitespace) between 15 and 5 characters before <paramref name="targetIndex"/>.
    /// </remarks>
    private static int GetStartIndexOfPhraseToShowBeforeTheTargetIndex(string value, int targetIndex)
    {
        const int defaultCharactersToKeep = 10;
        const int minCharactersToKeep = 5;
        const int maxCharactersToKeep = 15;
        const int lengthOfWhitespace = 1;
        const int phraseLengthToCheckForWordBoundary = (maxCharactersToKeep - minCharactersToKeep) + lengthOfWhitespace;

        if (targetIndex <= defaultCharactersToKeep)
        {
            return 0;
        }

        var indexToStartSearchingForWordBoundary = Math.Max(targetIndex - (maxCharactersToKeep + lengthOfWhitespace), 0);

        var indexOfWordBoundary = value
                .IndexOf(' ', indexToStartSearchingForWordBoundary, phraseLengthToCheckForWordBoundary) -
            indexToStartSearchingForWordBoundary;

        if (indexOfWordBoundary >= 0)
        {
            return indexToStartSearchingForWordBoundary + indexOfWordBoundary + lengthOfWhitespace;
        }

        return targetIndex - defaultCharactersToKeep;
    }

    /// <summary>
    /// Calculates how many characters to keep in <paramref name="value"/>.
    /// </summary>
    /// <remarks>
    /// If a word end is found between the configured string print length +10 or -5, use this word end, otherwise keep the exact character count.
    /// </remarks>
    private int GetLengthOfPhraseToShowOrDefaultLength(string value)
    {
        int minLength = stringPrintLength - 5;
        int maxLength = stringPrintLength + 10;
        const int lengthOfWhitespace = 1;

        var indexOfWordBoundary = value
            .LastIndexOf(' ', Math.Min(maxLength + lengthOfWhitespace, value.Length) - 1);

        if (indexOfWordBoundary >= minLength)
        {
            return indexOfWordBoundary;
        }

        return Math.Min(stringPrintLength, value.Length);
    }
}
