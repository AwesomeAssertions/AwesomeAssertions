using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AwesomeAssertions.Common;
using AwesomeAssertions.Execution;

namespace AwesomeAssertions.Primitives;

internal class StringEqualityStrategy : IStringComparisonStrategy
{
    private const string Indentation = "  ";
    private const string Prefix = Indentation + "\"";
    private const string Suffix = "\"";
    private const char ArrowDown = '\u2193';
    private const char ArrowUp = '\u2191';
    private const char Ellipsis = '\u2026';

    private readonly IEqualityComparer<string> comparer;
    private readonly string predicateDescription;

    public StringEqualityStrategy(IEqualityComparer<string> comparer, string predicateDescription)
    {
        this.comparer = comparer;
        this.predicateDescription = predicateDescription;
    }

    public string ExpectationDescription => $"Expected {{context:string}} to {predicateDescription} ";

    public void ValidateAgainstMismatch(AssertionChain assertionChain, string subject, string expected)
    {
        if (ValidateAgainstSuperfluousWhitespace(assertionChain, subject, expected))
        {
            return;
        }

        int indexOfMismatch = GetIndexOfFirstMismatch(subject, expected);

        assertionChain
            .ForCondition(indexOfMismatch == -1)
            .FailWith(() => new FailReason(CreateFailureMessage(subject, expected, indexOfMismatch)));
    }

    private string CreateFailureMessage(string subject, string expected, int indexOfMismatch)
    {
        string locationDescription = $"at index {indexOfMismatch}";
        var matchingString = subject[..indexOfMismatch];
        int lineNumber = matchingString.Count(c => c == '\n');

        if (lineNumber > 0)
        {
            var indexOfLastNewlineBeforeMismatch = matchingString.LastIndexOf('\n');
            var column = matchingString.Length - indexOfLastNewlineBeforeMismatch;
            locationDescription = $"on line {lineNumber + 1} and column {column} (index {indexOfMismatch})";
        }

        string mismatchSegment = GetMismatchSegment(subject, expected, indexOfMismatch).EscapePlaceholders();

        return $$"""
            {{ExpectationDescription}}the same string{reason}, but they differ {{locationDescription}}:
            {{mismatchSegment}}.
            """;
    }

    private bool ValidateAgainstSuperfluousWhitespace(AssertionChain assertion, string subject, string expected)
    {
        assertion
            .ForCondition(!(expected.Length > subject.Length && comparer.Equals(expected.TrimEnd(), subject)))
            .FailWith($"{ExpectationDescription}{{0}}{{reason}}, but it misses some extra whitespace at the end.", expected)
            .Then
            .ForCondition(!(subject.Length > expected.Length && comparer.Equals(subject.TrimEnd(), expected)))
            .FailWith($"{ExpectationDescription}{{0}}{{reason}}, but it has unexpected whitespace at the end.", expected);

        return !assertion.Succeeded;
    }

    /// <summary>
    /// Get the mismatch segment between <paramref name="expected"/> and <paramref name="subject"/>,
    /// when they differ at index <paramref name="firstIndexOfMismatch"/>.
    /// </summary>
    private static string GetMismatchSegment(string subject, string expected, int firstIndexOfMismatch)
    {
        int trimStart = GetStartIndexOfPhraseToShowBeforeTheMismatchingIndex(subject, firstIndexOfMismatch);

        int whiteSpaceCountBeforeArrow = (firstIndexOfMismatch - trimStart) + Prefix.Length;

        if (trimStart > 0)
        {
            whiteSpaceCountBeforeArrow++;
        }

        var visibleText = subject[trimStart..firstIndexOfMismatch];
        whiteSpaceCountBeforeArrow += visibleText.Count(c => c is '\r' or '\n');

        var sb = new StringBuilder();

        sb.Append(' ', whiteSpaceCountBeforeArrow).Append(ArrowDown).AppendLine(" (actual)");
        AppendPrefixAndEscapedPhraseToShowWithEllipsisAndSuffix(sb, subject, trimStart);
        AppendPrefixAndEscapedPhraseToShowWithEllipsisAndSuffix(sb, expected, trimStart);
        sb.Append(' ', whiteSpaceCountBeforeArrow).Append(ArrowUp).Append(" (expected)");

        return sb.ToString();
    }

    /// <summary>
    /// Appends the prefix, the escaped visible <paramref name="text"/> phrase decorated with ellipsis and the suffix to the <paramref name="stringBuilder"/>.
    /// </summary>
    /// <remarks>When text phrase starts at <paramref name="indexOfStartingPhrase"/> and with a calculated length omits text on start or end, an ellipsis is added.</remarks>
    private static void AppendPrefixAndEscapedPhraseToShowWithEllipsisAndSuffix(StringBuilder stringBuilder,
        string text, int indexOfStartingPhrase)
    {
        var subjectLength = GetLengthOfPhraseToShowOrDefaultLength(text[indexOfStartingPhrase..]);

        stringBuilder.Append(Prefix);

        if (indexOfStartingPhrase > 0)
        {
            stringBuilder.Append(Ellipsis);
        }

        stringBuilder.Append(text
            .Substring(indexOfStartingPhrase, subjectLength)
            .Replace("\r", "\\r", StringComparison.OrdinalIgnoreCase)
            .Replace("\n", "\\n", StringComparison.OrdinalIgnoreCase));

        if (text.Length > (indexOfStartingPhrase + subjectLength))
        {
            stringBuilder.Append(Ellipsis);
        }

        stringBuilder.AppendLine(Suffix);
    }

    /// <summary>
    /// Calculates the start index of the visible segment from <paramref name="value"/> when highlighting the difference at <paramref name="indexOfFirstMismatch"/>.
    /// </summary>
    /// <remarks>
    /// Either keep the last 10 characters before <paramref name="indexOfFirstMismatch"/> or a word begin (separated by whitespace) between 15 and 5 characters before <paramref name="indexOfFirstMismatch"/>.
    /// </remarks>
    private static int GetStartIndexOfPhraseToShowBeforeTheMismatchingIndex(string value, int indexOfFirstMismatch)
    {
        const int defaultCharactersToKeep = 10;
        const int minCharactersToKeep = 5;
        const int maxCharactersToKeep = 15;
        const int lengthOfWhitespace = 1;
        const int phraseLengthToCheckForWordBoundary = (maxCharactersToKeep - minCharactersToKeep) + lengthOfWhitespace;

        if (indexOfFirstMismatch <= defaultCharactersToKeep)
        {
            return 0;
        }

        var indexToStartSearchingForWordBoundary = Math.Max(indexOfFirstMismatch - (maxCharactersToKeep + lengthOfWhitespace), 0);

        var indexOfWordBoundary = value
                .IndexOf(' ', indexToStartSearchingForWordBoundary, phraseLengthToCheckForWordBoundary) -
            indexToStartSearchingForWordBoundary;

        if (indexOfWordBoundary >= 0)
        {
            return indexToStartSearchingForWordBoundary + indexOfWordBoundary + lengthOfWhitespace;
        }

        return indexOfFirstMismatch - defaultCharactersToKeep;
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

    /// <summary>
    /// Get index of the first mismatch between <paramref name="subject"/> and <paramref name="expected"/>. 
    /// </summary>
    /// <param name="subject"></param>
    /// <param name="expected"></param>
    /// <returns>Returns the index of the first mismatch, or -1 if the strings are equal.</returns>
    private int GetIndexOfFirstMismatch(string subject, string expected)
    {
        int indexOfMismatch = subject.IndexOfFirstMismatch(expected, comparer);

        if (indexOfMismatch != -1)
        {
            return indexOfMismatch;
        }

        // If no mismatch is found, we can assume the strings are equal when they also have the same length.
        if (subject.Length == expected.Length)
        {
            return -1;
        }

        // the mismatch is the first character of the longer string.
        return Math.Min(subject.Length, expected.Length);
    }
}
