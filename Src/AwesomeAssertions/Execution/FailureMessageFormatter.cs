using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using AwesomeAssertions.Common;
using AwesomeAssertions.Formatting;

namespace AwesomeAssertions.Execution;

/// <summary>
/// Encapsulates expanding the various placeholders supported in a failure message.
/// </summary>
internal class FailureMessageFormatter(FormattingOptions formattingOptions)
{
    private static readonly char[] Blanks = ['\r', '\n', ' ', '\t'];
    private string reason;
    private ContextDataDictionary contextData;
    private string identifier;
    private string fallbackIdentifier;

    public FailureMessageFormatter WithReason(string reason)
    {
        this.reason = reason ?? string.Empty;
        return this;
    }

    // SMELL: looks way too complex just to retain the leading whitespace
    private static string EnsurePrefix(string prefix, string text)
    {
        string leadingBlanks = ExtractLeadingBlanksFrom(text);
        string textWithoutLeadingBlanks = text.Substring(leadingBlanks.Length);

        return !textWithoutLeadingBlanks.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
            ? leadingBlanks + prefix + " " + textWithoutLeadingBlanks
            : text;
    }

    private static string ExtractLeadingBlanksFrom(string text)
    {
        string trimmedText = text.TrimStart(Blanks);
        int leadingBlanksCount = text.Length - trimmedText.Length;

        return text.Substring(0, leadingBlanksCount);
    }

    private static bool StartsWithBlank(string text)
    {
        return text.Length > 0 && Blanks.Contains(text[0]);
    }

    public FailureMessageFormatter WithContext(ContextDataDictionary contextData)
    {
        this.contextData = contextData;
        return this;
    }

    public FailureMessageFormatter WithIdentifier(string identifier)
    {
        this.identifier = identifier;
        return this;
    }

    public FailureMessageFormatter WithFallbackIdentifier(string fallbackIdentifier)
    {
        this.fallbackIdentifier = fallbackIdentifier;
        return this;
    }

    public string Format(string message, object[] messageArgs)
    {
        message = SubstituteReason(message);

        message = SubstituteIdentifier(message, identifier?.EscapePlaceholders(), fallbackIdentifier);

        message = SubstituteContextualTags(message, contextData);

        message = FormatArgumentPlaceholders(message, messageArgs);

        return message;
    }

    /// <summary>
    /// Substitutes the {reason} placeholder in the message template.
    /// </summary>
    /// <param name="message">The message template to parse.</param>
    /// <returns>The changed message template.</returns>
    private string SubstituteReason(string message)
    {
        int indexOfReason = message.IndexOf("{reason}", StringComparison.Ordinal);
        if (indexOfReason < 0)
        {
            return message;
        }

        bool isPreceededByNewLine = indexOfReason > 0 && (message[indexOfReason - 1] == '\n' || message[indexOfReason - 1] == '\r');

        if (string.IsNullOrEmpty(reason) && isPreceededByNewLine)
        {
            return RemoveEmptyReasonAtLineStart(message, indexOfReason);
        }

        string sanitizedReason = SanitizeReason(addBlank: !isPreceededByNewLine);
        return message.Replace("{reason}", sanitizedReason, StringComparison.Ordinal);
    }

    /// <summary>
    /// For multiline messages, where we have a line break before the reason,
    /// we want to remove a trailing comma and space if the reason is empty
    /// </summary>
    /// <param name="message">The original message with {reason} template.</param>
    /// <param name="indexOfReason">Index of the {reason} placeholder within <paramref name="message"/>.</param>
    /// <returns>The message without reason</returns>
    private static string RemoveEmptyReasonAtLineStart(string message, int indexOfReason)
    {
        int placeholderLength = "{reason}".Length;
        int indexOfComma = indexOfReason + placeholderLength;
        int indexOfSpace = indexOfComma + 1;
        int removeCount = placeholderLength;
        if (message.Length > indexOfSpace &&
            message[indexOfComma] == ',' && message[indexOfSpace] == ' ')
        {
            removeCount += 2; // remove the comma and the space
        }

        return message.Remove(indexOfReason, removeCount);
    }

    private string SanitizeReason(bool addBlank)
    {
        if (string.IsNullOrEmpty(reason))
        {
            return string.Empty;
        }

        string sanitizedReason = EnsurePrefix("because", reason);
        sanitizedReason = sanitizedReason.EscapePlaceholders();

        return (StartsWithBlank(sanitizedReason) || !addBlank) ? sanitizedReason : " " + sanitizedReason;
    }

    private static string SubstituteIdentifier(string message, string identifier, string fallbackIdentifier)
    {
        const string pattern = @"(?:\s|^)\{context(?:\:(?<default>[a-zA-Z\s]+))?\}";

        message = Regex.Replace(message, pattern, match =>
        {
            const string result = " ";

            if (!string.IsNullOrEmpty(identifier))
            {
                return result + identifier;
            }

            string defaultIdentifier = match.Groups["default"].Value;

            if (!string.IsNullOrEmpty(defaultIdentifier))
            {
                return result + defaultIdentifier;
            }

            if (!string.IsNullOrEmpty(fallbackIdentifier))
            {
                return result + fallbackIdentifier;
            }

            return " object";
        });

        return message.TrimStart();
    }

    private static string SubstituteContextualTags(string message, ContextDataDictionary contextData)
    {
        const string pattern = @"(?<!\{)\{(?<key>[a-zA-Z]+)(?:\:(?<default>[a-zA-Z\s]+))?\}(?!\})";

        return Regex.Replace(message, pattern, match =>
        {
            string key = match.Groups["key"].Value;
            string contextualTags = contextData.AsStringOrDefault(key);
            string contextualTagsSubstituted = contextualTags?.EscapePlaceholders();

            return contextualTagsSubstituted ?? match.Groups["default"].Value;
        });
    }

    private string FormatArgumentPlaceholders(string failureMessage, object[] failureArgs)
    {
        object[] values = failureArgs.Select(object (a) => Formatter.ToString(a, formattingOptions)).ToArray();

        try
        {
            return string.Format(CultureInfo.InvariantCulture, failureMessage, values);
        }
        catch (FormatException formatException)
        {
            return
                $"**WARNING** failure message '{failureMessage}' could not be formatted with string.Format{Environment.NewLine}{formatException.StackTrace}";
        }
    }
}
