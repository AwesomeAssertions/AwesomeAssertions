using System;
using System.Text;

namespace FluentAssertions.Formatting
{
    /// <summary>
    /// Extension method for formatting
    /// </summary>
    internal static class FormattingExtensions
    {
        /// <summary>
        /// Appends type formatted to a friendly name.
        /// </summary>
        /// <param name="sb">The target string builder.</param>
        /// <param name="type">The type to format.</param>
        /// <returns>The string builder for a fluent api</returns>
        public static StringBuilder AppendFormatted(this StringBuilder sb, Type type)
        {
            TypeValueFormatter.Format(type, sb);
            return sb;
        }

        /// <summary>
        /// Appends type formatted to a friendly name without its leading namespace.
        /// </summary>
        /// <remarks>
        /// Namespaces of other types, e.g. generic arguments, are still added.
        /// </remarks>
        /// <param name="sb">The target string builder.</param>
        /// <param name="type">The type to format.</param>
        /// <returns>The string builder for a fluent api</returns>
        public static StringBuilder AppendFormattedWithoutLeadingNamespace(this StringBuilder sb, Type type)
        {
            TypeValueFormatter.FormatWithoutLeadingNamespace(type, sb);
            return sb;
        }
    }
}
