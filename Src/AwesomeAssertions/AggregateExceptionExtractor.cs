using System;
using System.Collections.Generic;
using System.Linq;
using AwesomeAssertions.Common;
using AwesomeAssertions.Specialized;

namespace AwesomeAssertions;

/// <summary>
/// Extracts the exceptions of a particular type from an <see cref="AggregateException"/> (or a single exception).
/// </summary>
public class AggregateExceptionExtractor : IExtractExceptions
{
    /// <summary>
    /// Extracts all exceptions of type <typeparamref name="T"/> from the specified exception.
    /// </summary>
    /// <typeparam name="T">The type of the exceptions to extract.</typeparam>
    /// <param name="actualException">
    /// The exception to extract from. When it is an <see cref="AggregateException"/>, its flattened inner
    /// exceptions are searched; otherwise the exception itself is considered.
    /// </param>
    /// <returns>The exceptions of type <typeparamref name="T"/> that were found.</returns>
    public IEnumerable<T> OfType<T>(Exception actualException)
        where T : Exception
    {
        if (typeof(T).IsSameOrInherits(typeof(AggregateException)))
        {
            return actualException is T exception ? [exception] : [];
        }

        return GetExtractedExceptions<T>(actualException);
    }

    private static List<T> GetExtractedExceptions<T>(Exception actualException)
        where T : Exception
    {
        var exceptions = new List<T>();

        if (actualException is AggregateException aggregateException)
        {
            AggregateException flattenedExceptions = aggregateException.Flatten();

            exceptions.AddRange(flattenedExceptions.InnerExceptions.OfType<T>());
        }
        else if (actualException is T genericException)
        {
            exceptions.Add(genericException);
        }

        return exceptions;
    }
}
