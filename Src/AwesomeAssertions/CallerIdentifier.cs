using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using AwesomeAssertions.CallerIdentification;
using AwesomeAssertions.Common;

namespace AwesomeAssertions;

/// <summary>
/// Tries to extract the name of the variable or invocation on which the assertion is executed.
/// </summary>
// REFACTOR: Should be internal and treated as an implementation detail of the AssertionScope
public static class CallerIdentifier
{
    public static Action<string> Logger { get; set; } = _ => { };

    public static string DetermineCallerIdentity()
    {
        string caller = null;

        try
        {
            var stack = new StackTrace(fNeedFileInfo: true);

            var allStackFrames = GetFrames(stack);

            int searchStart = allStackFrames.Length - 1;

            if (StartStackSearchAfterStackFrame.Value is not null)
            {
                searchStart = Array.FindLastIndex(
                    allStackFrames,
                    allStackFrames.Length - StartStackSearchAfterStackFrame.Value.SkipStackFrameCount,
                    frame => !IsCurrentAssembly(frame));
            }

            int lastUserStackFrameBeforeAwesomeAssertionsCodeIndex = Array.FindIndex(
                allStackFrames,
                startIndex: 0,
                count: searchStart + 1,
                frame => !IsCurrentAssembly(frame) && !IsDynamic(frame) && !IsDotNet(frame));

            for (int i = lastUserStackFrameBeforeAwesomeAssertionsCodeIndex; i < allStackFrames.Length; i++)
            {
                var frame = allStackFrames[i];

                Logger(frame.ToString());

                if (frame.GetMethod() is not null
                    && !IsDynamic(frame)
                    && !IsDotNet(frame)
                    && !IsCustomAssertion(frame)
                    && !IsCurrentAssembly(frame))
                {
                    caller = ExtractVariableNameFrom(frame);
                    break;
                }
            }
        }
        catch (Exception e)
        {
            // Ignore exceptions, as determination of caller identity is only a nice-to-have
            Logger(e.ToString());
        }

        return caller;
    }

    private sealed class StackFrameReference : IDisposable
    {
        public int SkipStackFrameCount { get; }

        private readonly StackFrameReference previousReference;

        public StackFrameReference()
        {
            var stack = new StackTrace();

            var allStackFrames = GetFrames(stack);

            int firstUserCodeFrameIndex = 0;

            while (firstUserCodeFrameIndex < allStackFrames.Length
                   && IsCurrentAssembly(allStackFrames[firstUserCodeFrameIndex]))
            {
                firstUserCodeFrameIndex++;
            }

            SkipStackFrameCount = (allStackFrames.Length - firstUserCodeFrameIndex) + 1;

            previousReference = StartStackSearchAfterStackFrame.Value;
            StartStackSearchAfterStackFrame.Value = this;
        }

        public void Dispose()
        {
            StartStackSearchAfterStackFrame.Value = previousReference;
        }
    }

    private static readonly AsyncLocal<StackFrameReference> StartStackSearchAfterStackFrame = new();

    internal static IDisposable OverrideStackSearchUsingCurrentScope()
    {
        return new StackFrameReference();
    }

    internal static bool OnlyOneAssertionScopeOnCallStack()
    {
        var allStackFrames = GetFrames(new StackTrace());

        int firstNonAwesomeAssertionsStackFrameIndex = Array.FindIndex(
            allStackFrames,
            frame => !IsCurrentAssembly(frame));

        if (firstNonAwesomeAssertionsStackFrameIndex < 0)
        {
            return true;
        }

        int startOfSecondAwesomeAssertionsScopeStackFrameIndex = Array.FindIndex(
            allStackFrames,
            startIndex: firstNonAwesomeAssertionsStackFrameIndex + 1,
            frame => IsCurrentAssembly(frame));

        return startOfSecondAwesomeAssertionsScopeStackFrameIndex < 0;
    }

    private static bool IsCustomAssertion(StackFrame frame)
    {
        MethodBase getMethod = frame.GetMethod();

        if (getMethod is not null)
        {
            return
                getMethod.IsDecoratedWithOrInherit<CustomAssertionAttribute>() ||
                IsCustomAssertionClass(getMethod.DeclaringType) ||
                getMethod.ReflectedType?.Assembly.IsDefined(typeof(CustomAssertionsAssemblyAttribute)) == true;
        }

        return false;
    }

    /// <summary>
    /// Check if <paramref name="type"/> is a custom assertion class.
    /// </summary>
    /// <remarks>
    /// Checking also explicitly the declaring type is necessary for DisplayClasses, which can't be marked
    /// themselves like normal nested classes.
    /// </remarks>
    /// <param name="type">The class type to check</param>
    /// <returns>True if type is a custom assertion class, or nested inside such a class.</returns>
    private static bool IsCustomAssertionClass(Type type)
        => type is not null && (type.IsDecoratedWithOrInherit<CustomAssertionsAttribute>() || IsCustomAssertionClass(type.DeclaringType));

    private static bool IsDynamic(StackFrame frame)
    {
        return frame.GetMethod() is { DeclaringType: null };
    }

    private static bool IsCurrentAssembly(StackFrame frame)
    {
        return frame.GetMethod()?.DeclaringType?.Assembly == typeof(CallerIdentifier).Assembly;
    }

    private static bool IsDotNet(StackFrame frame)
    {
        var frameNamespace = frame.GetMethod()?.DeclaringType?.Namespace;
        const StringComparison comparisonType = StringComparison.OrdinalIgnoreCase;

        return frameNamespace?.StartsWith("system.", comparisonType) == true ||
            frameNamespace?.Equals("system", comparisonType) == true;
    }

    private static bool IsCompilerServices(StackFrame frame)
    {
        return frame.GetMethod()?.DeclaringType?.Namespace is "System.Runtime.CompilerServices";
    }

    private static string ExtractVariableNameFrom(StackFrame frame)
    {
        string caller = null;
        string statement = GetSourceCodeStatementFrom(frame);

        if (!string.IsNullOrEmpty(statement))
        {
            Logger(statement);

            if (!IsBooleanLiteral(statement) && !IsNumeric(statement) && !IsStringLiteral(statement) &&
                !StartsWithNewKeyword(statement))
            {
                caller = statement;
            }
        }

        return caller;
    }

    private static string GetSourceCodeStatementFrom(StackFrame frame)
    {
        string fileName = frame.GetFileName();
        int expectedLineNumber = frame.GetFileLineNumber();

        if (string.IsNullOrEmpty(fileName) || expectedLineNumber == 0)
        {
            return null;
        }

        try
        {
            using var reader = new StreamReader(File.OpenRead(fileName));
            string line;
            int currentLine = 1;

            while ((line = reader.ReadLine()) is not null && currentLine < expectedLineNumber)
            {
                currentLine++;
            }

            return currentLine == expectedLineNumber
                && line != null
                    ? GetSourceCodeStatementFrom(frame, reader, line)
                    : null;
        }
        catch
        {
            // We don't care. Just assume the symbol file is not available or unreadable
            return null;
        }
    }

    private static string GetSourceCodeStatementFrom(StackFrame frame, StreamReader reader, string line)
    {
        int column = frame.GetFileColumnNumber();

        if (column > 0)
        {
            line = line.Substring(Math.Min(column - 1, line.Length - 1));
        }

        var sb = new CallerStatementBuilder();

        do
        {
            sb.Append(line);
        }
        while (!sb.IsDone() && (line = reader.ReadLine()) != null);

        return sb.ToString();
    }

    private static bool StartsWithNewKeyword(string candidate)
    {
        return Regex.IsMatch(candidate, @"(?:^|s+)new(?:\s?\[|\s?\{|\s\w+)");
    }

    private static bool IsStringLiteral(string candidate)
    {
        return candidate.StartsWith('\"');
    }

    private static bool IsNumeric(string candidate)
    {
        const NumberStyles numberStyle = NumberStyles.Float | NumberStyles.AllowThousands;
        return double.TryParse(candidate, numberStyle, CultureInfo.InvariantCulture, out _);
    }

    private static bool IsBooleanLiteral(string candidate)
    {
        return candidate is "true" or "false";
    }

    private static StackFrame[] GetFrames(StackTrace stack)
    {
        var frames = stack.GetFrames();
#if !NET6_0_OR_GREATER
        if (frames == null)
        {
            return [];
        }
#endif
        return frames
            .Where(frame => !IsCompilerServices(frame))
            .ToArray();
    }
}
