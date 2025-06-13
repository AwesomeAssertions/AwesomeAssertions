using System;
using System.Globalization;

namespace AwesomeAssertions.Extensions;

internal static class StringExtensions
{
    public static string FirstCharToLower(this string str) =>
#pragma warning disable CA1308
        string.Concat(str[0].ToString().ToLowerInvariant(), str.AsSpan(1));
#pragma warning restore CA1308
}
