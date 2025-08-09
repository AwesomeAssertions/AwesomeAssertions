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

    public static string RenderMessage(MismatchContext context)
    {
        var mismatchSegment = GetMismatchSegment(context).EscapePlaceholders();
        var prefix = context.IsStringEndStrategy ? "before" : "at";

        var locationDescription =
            CreateLocationDescription(context.Subject, context.IndexOfMismatch, prefix);

        return CreateMessage(context.ExpectationDescription, locationDescription, mismatchSegment);
    }

    private static string CreateLocationDescription(string subject, int indexOfMismatch, string prefix)
    {
        var matchingString = subject[..indexOfMismatch];
        int lineNumber = matchingString.Count(c => c == '\n');

        if (lineNumber > 0)
        {
            var indexOfLastNewlineBeforeMismatch = matchingString.LastIndexOf('\n');
            var column = matchingString.Length - indexOfLastNewlineBeforeMismatch;
            return $"on line {lineNumber + 1} and column {column} (index {indexOfMismatch})";
        }

        return $"{prefix} index {indexOfMismatch}";
    }

    private static string CreateMessage(string expectationDescription, string locationDescription, string mismatchSegment)
    {
        return $$"""
                 {{expectationDescription}}the same string{reason}, but they differ {{locationDescription}}:
                 {{mismatchSegment}}.
                 """;
    }

    /// <summary>
    /// Get the mismatch segment between expected and subject,
    /// when they differ at index firstIndexOfMismatch.
    /// </summary>
    private static string GetMismatchSegment(MismatchContext context)
    {
        var subject = context.SubjectVisibleSpan;
        var expected = context.ExpectedVisibleSpan;
        var isStringEndStrategy = context.IsStringEndStrategy;

        AppendPrefixAndEscapedPhraseToShowWithEllipsisAndSuffix(subject);
        AppendPrefixAndEscapedPhraseToShowWithEllipsisAndSuffix(expected);

        var (shorterSpan, longerSpan) = subject.Length >= expected.Length
            ? (expected, subject)
            : (subject, expected);

        if (isStringEndStrategy)
        {
            shorterSpan.Prepend(new string(' ', longerSpan.Length - shorterSpan.Length));
        }

        int whiteSpaceCountBeforeArrow = longerSpan.MismatchIndex;

        return new StringBuilder()
            .Append(' ', whiteSpaceCountBeforeArrow)
            .AppendLine($"{ArrowDown} (actual)")
            .AppendLine(subject.Text)
            .AppendLine(expected.Text)
            .Append(' ', whiteSpaceCountBeforeArrow)
            .Append($"{ArrowUp} (expected)")
            .ToString();
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
