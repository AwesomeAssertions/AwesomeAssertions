using System;
using System.Text.RegularExpressions;

namespace AwesomeAssertions.Equivalency.Ordering;

/// <summary>
/// Represents a rule for determining whether or not a certain collection within the object graph should be compared using
/// strict ordering.
/// </summary>
internal class PathBasedOrderingRule : IOrderingRule
{
    private readonly string path;

    public PathBasedOrderingRule(string path)
    {
        this.path = path;
    }

    public bool Invert { get; init; }

    /// <summary>
    /// Determines if ordering of the member referred to by the current <paramref name="objectInfo"/> is relevant.
    /// </summary>
    public OrderStrictness Evaluate(IObjectInfo objectInfo)
    {
        string currentPropertyPath = objectInfo.Path;

        if (!ContainsIndexingQualifiers(path))
        {
            currentPropertyPath = RemoveInitialIndexQualifier(currentPropertyPath);
        }

        if (currentPropertyPath.Equals(path, StringComparison.OrdinalIgnoreCase))
        {
            return Invert ? OrderStrictness.NotStrict : OrderStrictness.Strict;
        }

        return OrderStrictness.Irrelevant;
    }

    private static bool ContainsIndexingQualifiers(string path)
    {
        return path.Contains('[', StringComparison.Ordinal) && path.Contains(']', StringComparison.Ordinal);
    }

    private string RemoveInitialIndexQualifier(string sourcePath)
    {
        var indexQualifierRegex = new Regex(@"^\[[0-9]+]\.");

        if (!indexQualifierRegex.IsMatch(path))
        {
            Match match = indexQualifierRegex.Match(sourcePath);

            if (match.Success)
            {
                sourcePath = sourcePath.Substring(match.Length);
            }
        }

        return sourcePath;
    }

    public override string ToString()
    {
        return $"Be {(Invert ? "not strict" : "strict")} about the order of collection items when path is " + path;
    }
}
