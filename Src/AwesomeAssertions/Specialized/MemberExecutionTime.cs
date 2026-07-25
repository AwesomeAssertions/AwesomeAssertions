using System;
using System.Linq.Expressions;
using AwesomeAssertions.Common;

namespace AwesomeAssertions.Specialized;

/// <summary>
/// Captures the time it takes to execute a method or property of a subject of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the subject that exposes the member being measured.</typeparam>
public class MemberExecutionTime<T> : ExecutionTime
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MemberExecutionTime{T}"/> class.
    /// </summary>
    /// <param name="subject">The object that exposes the method or property.</param>
    /// <param name="action">A reference to the method or property to measure the execution time of.</param>
    /// <param name="createTimer">A function that creates the <see cref="ITimer"/> used to measure the elapsed time.</param>
    /// <exception cref="NullReferenceException"><paramref name="subject"/> is <see langword="null"/>.</exception>
    /// <exception cref="NullReferenceException"><paramref name="action"/> is <see langword="null"/>.</exception>
    public MemberExecutionTime(T subject, Expression<Action<T>> action, StartTimer createTimer)
        : base(() => action.Compile()(subject), "(" + action.Body + ")", createTimer)
    {
    }
}
