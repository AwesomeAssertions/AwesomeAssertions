using System;
using System.Collections.Generic;
using System.Linq;
using AwesomeAssertions.Common;
using AwesomeAssertions.Specialized;

namespace AwesomeAssertions;

public class AggregateExceptionExtractor : IExtractExceptions
{
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
