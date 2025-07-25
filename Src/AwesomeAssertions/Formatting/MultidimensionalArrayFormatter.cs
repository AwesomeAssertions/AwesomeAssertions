using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace AwesomeAssertions.Formatting;

public class MultidimensionalArrayFormatter : IValueFormatter
{
    /// <summary>
    /// Indicates whether the current <see cref="IValueFormatter"/> can handle the specified <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The value for which to create a <see cref="string"/>.</param>
    /// <returns>
    /// <see langword="true"/> if the current <see cref="IValueFormatter"/> can handle the specified value; otherwise, <see langword="false"/>.
    /// </returns>
    public bool CanHandle(object value)
    {
        return value is Array { Rank: >= 2 };
    }

    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "Required refactoring")]
    public void Format(object value, FormattedObjectGraph formattedGraph, FormattingContext context, FormatChild formatChild)
    {
        var arr = (Array)value;

        if (arr.Length == 0)
        {
            formattedGraph.AddFragment("{empty}");
            return;
        }

        int[] dimensionIndices = Enumerable.Range(0, arr.Rank).Select(dimension => arr.GetLowerBound(dimension)).ToArray();

        int currentLoopIndex = 0;

        // ReSharper disable once NotDisposedResource (false positive)
        // See https://youtrack.jetbrains.com/issue/QD-8114/It-is-not-a-problem-Local-variable-enumerator-is-never-disposed
        IEnumerator enumerator = arr.GetEnumerator();

        // Emulate n-ary loop
        while (currentLoopIndex >= 0)
        {
            int currentDimensionIndex = dimensionIndices[currentLoopIndex];

            if (IsFirstIteration(arr, currentDimensionIndex, currentLoopIndex))
            {
                formattedGraph.AddFragment("{");
            }

            if (IsInnerMostLoop(arr, currentLoopIndex))
            {
                enumerator.MoveNext();
                formatChild(string.Join("-", dimensionIndices), enumerator.Current, formattedGraph);

                if (!IsLastIteration(arr, currentDimensionIndex, currentLoopIndex))
                {
                    formattedGraph.AddFragment(", ");
                }

                ++dimensionIndices[currentLoopIndex];
            }
            else
            {
                ++currentLoopIndex;
                continue;
            }

            while (IsLastIteration(arr, currentDimensionIndex, currentLoopIndex))
            {
                formattedGraph.AddFragment("}");

                // Reset current loop's variable to start value ...and move to outer loop
                dimensionIndices[currentLoopIndex] = arr.GetLowerBound(currentLoopIndex);
                --currentLoopIndex;

                if (currentLoopIndex < 0)
                {
                    break;
                }

                currentDimensionIndex = dimensionIndices[currentLoopIndex];

                if (!IsLastIteration(arr, currentDimensionIndex, currentLoopIndex))
                {
                    formattedGraph.AddFragment(", ");
                }

                ++dimensionIndices[currentLoopIndex];
            }
        }
    }

    private static bool IsFirstIteration(Array arr, int index, int dimension)
    {
        return index == arr.GetLowerBound(dimension);
    }

    private static bool IsInnerMostLoop(Array arr, int index)
    {
        return index == (arr.Rank - 1);
    }

    private static bool IsLastIteration(Array arr, int index, int dimension)
    {
        return index >= arr.GetUpperBound(dimension);
    }
}
