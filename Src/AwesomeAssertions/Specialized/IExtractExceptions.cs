using System;
using System.Collections.Generic;

namespace AwesomeAssertions.Specialized;

/// <summary>
/// Defines a strategy for extracting exceptions of a specific type from a thrown exception.
/// </summary>
public interface IExtractExceptions
{
    /// <summary>
    /// Extracts the exceptions of type <typeparamref name="T"/> from the specified <paramref name="actualException"/>.
    /// </summary>
    /// <typeparam name="T">The type of exception to extract.</typeparam>
    /// <param name="actualException">The exception that was actually thrown.</param>
    /// <returns>The exceptions of type <typeparamref name="T"/> that were found.</returns>
    IEnumerable<T> OfType<T>(Exception actualException)
        where T : Exception;
}
