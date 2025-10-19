using System.Linq;
using System.Text;

namespace AwesomeAssertions.Common.Mismatch;

/// <summary>
/// Provides functionality for rendering failure messages that describe mismatches between
/// actual and expected string values, including contextual details such as the location
/// of the mismatch and formatted descriptions.
/// </summary>
internal static class MismatchRenderer
{
    private const string Indentation = "  ";
    private const string Prefix = Indentation + "\"";
    private const string Suffix = "\"";
    private const string ArrowDown = "\u2193";
    private const string ArrowUp = "\u2191";
    private const string Ellipsis = "\u2026";
    private const string ActualMarker = $"{ArrowDown} (actual)";
    private const string ExpectedMarker = $"{ArrowUp} (expected)";

    /// <summary>
    /// Creates a failure message showing where the mismatch occurred between the two strings.
    /// </summary>
    public static string CreateFailureMessage(MismatchRendererOptions rendererOptions)
    {
        var mismatchSegment = GetMismatchSegment(rendererOptions).EscapePlaceholders();

        var locationDescription =
            CreateLocationDescription(rendererOptions.Subject, rendererOptions.SubjectIndexOfMismatch, rendererOptions.MismatchLocationDescription);

        return $$"""
                 {{rendererOptions.ExpectationDescription}}the same string{reason}, but they differ {{locationDescription}}:
                 {{mismatchSegment}}.
                 """;
    }

    /// <summary>
    /// Creates the location description portion of the message.
    /// </summary>
    private static string CreateLocationDescription(string subject, int indexOfMismatch, string defaultMessage)
    {
        var matchingString = subject[..indexOfMismatch];
        int lineNumber = matchingString.Count(c => c == '\n');

        if (lineNumber > 0)
        {
            var indexOfLastNewlineBeforeMismatch = matchingString.LastIndexOf('\n');
            var column = matchingString.Length - indexOfLastNewlineBeforeMismatch;
            return $"on line {lineNumber + 1} and column {column} (index {indexOfMismatch})";
        }

        return defaultMessage;
    }

    /// <summary>
    /// Get the mismatch segment between expected and subject,
    /// when they differ at index firstIndexOfMismatch.
    /// </summary>
    private static string GetMismatchSegment(MismatchRendererOptions rendererOptions)
    {
        var subject = new MismatchSpan(rendererOptions.Subject, rendererOptions.SubjectIndexOfMismatch);
        var expected = new MismatchSpan(rendererOptions.Expected, rendererOptions.ExpectedIndexOfMismatch);
        var alignment = rendererOptions.Alignment;

        FormatSpan(subject);
        FormatSpan(expected);

        // Sometimes, the subject becomes longer than the expected as a result of the truncation algorithm, or vice versa.
        // We need to align them relative to the length of the longer one.
        var (shorterSpan, longerSpan) = subject.Length >= expected.Length
            ? (expected, subject)
            : (subject, expected);

        if (alignment is Alignment.Right)
        {
            shorterSpan.Prepend(new string(' ', longerSpan.Length - shorterSpan.Length));
        }

        // When they are aligned, they should have the same number of characters to the left of the mismatch.
        int whiteSpaceCountBeforeArrow = longerSpan.MismatchIndex;

        return new StringBuilder()
            .Append(' ', whiteSpaceCountBeforeArrow)
            .AppendLine(ActualMarker)
            .AppendLine(subject.Text)
            .AppendLine(expected.Text)
            .Append(' ', whiteSpaceCountBeforeArrow)
            .Append(ExpectedMarker)
            .ToString();
    }

    /// <summary>
    /// Formats the text span for display in the failure message.
    /// </summary>
    private static void FormatSpan(MismatchSpan span)
    {
        span.Truncate();
        span.EscapeWhiteSpaces();
        AppendPrefixAndEscapedPhraseToShowWithEllipsisAndSuffix(span);
    }

    /// <summary>
    /// Appends the prefix, the escaped visible <paramref name="span"/> decorated with ellipsis and the suffix to the <paramref name="span"/>.
    /// </summary>
    /// <remarks>Ellipses are added for truncated ends of the <paramref name="span"/>.</remarks>
    private static void AppendPrefixAndEscapedPhraseToShowWithEllipsisAndSuffix(MismatchSpan span)
    {
        var startTruncated = span.StartTruncated;
        var endTruncated = span.EndTruncated;

        if (startTruncated)
        {
            span.Prepend(Ellipsis);
        }

        if (endTruncated)
        {
            span.Append(Ellipsis);
        }

        span.Prepend(Prefix);
        span.Append(Suffix);
    }
}
