using System.Linq;
using System.Text;

namespace AwesomeAssertions.Common.Mismatch;

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

    public static string CreateFailureMessage(MismatchRendererOptions rendererOptions)
    {
        var mismatchSegment = GetMismatchSegment(rendererOptions).EscapePlaceholders();

        var defaultIndexMessage = rendererOptions.IndexFormatter(rendererOptions.SubjectIndexOfMismatch);
        var locationDescription =
            CreateLocationDescription(rendererOptions.Subject, rendererOptions.SubjectIndexOfMismatch, defaultIndexMessage);

        return $$"""
                 {{rendererOptions.ExpectationDescription}}the same string{reason}, but they differ {{locationDescription}}:
                 {{mismatchSegment}}.
                 """;
    }

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
        var subject = MismatchSpan.Create(rendererOptions.Subject, rendererOptions.SubjectIndexOfMismatch);
        var expected = MismatchSpan.Create(rendererOptions.Expected, rendererOptions.ExpectedIndexOfMismatch);
        var truncationStrategy = rendererOptions.TruncationStrategy;
        var alignRight = rendererOptions.AlignRight;

        FormatSpan(subject, truncationStrategy);
        FormatSpan(expected, truncationStrategy);

        var (shorterSpan, longerSpan) = subject.Length >= expected.Length
            ? (expected, subject)
            : (subject, expected);

        if (alignRight)
        {
            shorterSpan.Prepend(new string(' ', longerSpan.Length - shorterSpan.Length));
        }

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
    private static void FormatSpan(MismatchSpan span, ITruncationStrategy truncationStrategy)
    {
        span.Truncate(truncationStrategy);
        span.EscapeNewLines();
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
