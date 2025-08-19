using System;

namespace AwesomeAssertions.Common.Mismatch;

internal class StandardTruncationStrategy : ITruncationStrategy
{
    public Range GetTruncationRange(string text, int targetIndex)
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
    /// If a word end is found between 45 and 60 characters, use this word end, otherwise keep 50 characters.
    /// </remarks>
    private static int GetLengthOfPhraseToShowOrDefaultLength(string value)
    {
        var defaultLength = AssertionConfiguration.Current.Formatting.StringPrintLength;
        int minLength = defaultLength - 5;
        int maxLength = defaultLength + 10;
        const int lengthOfWhitespace = 1;

        var indexOfWordBoundary = value
            .LastIndexOf(' ', Math.Min(maxLength + lengthOfWhitespace, value.Length) - 1);

        if (indexOfWordBoundary >= minLength)
        {
            return indexOfWordBoundary;
        }

        return Math.Min(defaultLength, value.Length);
    }
}
