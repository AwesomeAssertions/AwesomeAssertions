using System;

namespace AwesomeAssertions.Formatting;

#pragma warning disable RCS1194, CA1032 // Add constructors
/// <summary>
/// Thrown when the formatted output exceeds the configured maximum number of lines.
/// </summary>
public class MaxLinesExceededException : Exception;
#pragma warning restore CA1032, RCS1194
