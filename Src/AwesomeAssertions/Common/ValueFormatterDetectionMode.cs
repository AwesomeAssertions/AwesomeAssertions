﻿using AwesomeAssertions.Configuration;

namespace AwesomeAssertions.Common;

/// <summary>
/// Defines the modes in which custom implementations of <see cref="AwesomeAssertions.Formatting.IValueFormatter"/>
/// are detected as configured through <see cref="GlobalFormattingOptions"/>.
/// </summary>
public enum ValueFormatterDetectionMode
{
    /// <summary>
    /// Detection is disabled.
    /// </summary>
    Disabled,

    /// <summary>
    /// Only custom value formatters exposed through the assembly set in <see cref="GlobalFormattingOptions.ValueFormatterAssembly"/>
    /// are detected.
    /// </summary>
    Specific,

    /// <summary>
    /// All custom value formatters in any assembly loaded in the current AppDomain will be detected.
    /// </summary>
    Scan,
}
