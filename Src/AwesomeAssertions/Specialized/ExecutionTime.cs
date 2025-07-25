using System;
using System.Threading.Tasks;
using AwesomeAssertions.Common;

namespace AwesomeAssertions.Specialized;

public class ExecutionTime
{
    private ITimer timer;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExecutionTime"/> class.
    /// </summary>
    /// <param name="action">The action of which the execution time must be asserted.</param>
    /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null"/>.</exception>
    public ExecutionTime(Action action, StartTimer createTimer)
        : this(action, "the action", createTimer)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExecutionTime"/> class.
    /// </summary>
    /// <param name="action">The action of which the execution time must be asserted.</param>
    /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null"/>.</exception>
    public ExecutionTime(Func<Task> action, StartTimer createTimer)
        : this(action, "the action", createTimer)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExecutionTime"/> class.
    /// </summary>
    /// <param name="action">The action of which the execution time must be asserted.</param>
    /// <param name="actionDescription">The description of the action to be asserted.</param>
    /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null"/>.</exception>
    protected ExecutionTime(Action action, string actionDescription, StartTimer createTimer)
    {
        Guard.ThrowIfArgumentIsNull(action);

        ActionDescription = actionDescription;
        IsRunning = true;

        Task = Task.Run(() =>
        {
            // move stopwatch as close to action start as possible
            // so that we have to get correct time readings
            try
            {
                using (timer = createTimer())
                {
                    action();
                }
            }
            catch (Exception exception)
            {
                Exception = exception;
            }
            finally
            {
                // ensures that we stop the stopwatch even on exceptions
                IsRunning = false;
            }
        });
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExecutionTime"/> class.
    /// </summary>
    /// <param name="action">The action of which the execution time must be asserted.</param>
    /// <param name="actionDescription">The description of the action to be asserted.</param>
    /// <remarks>
    /// This constructor is almost exact copy of the one accepting <see cref="Action"/>.
    /// The original constructor shall stay in place in order to keep backward-compatibility
    /// and to avoid unnecessary wrapping action in <see cref="Task"/>.
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null"/>.</exception>
    protected ExecutionTime(Func<Task> action, string actionDescription, StartTimer createTimer)
    {
        Guard.ThrowIfArgumentIsNull(action);

        ActionDescription = actionDescription;
        IsRunning = true;

        Task = Task.Run(async () =>
        {
            // move stopwatch as close to action start as possible
            // so that we have to get correct time readings
            try
            {
                using (timer = createTimer())
                {
                    await action();
                }
            }
            catch (Exception exception)
            {
                Exception = exception;
            }
            finally
            {
                IsRunning = false;
            }
        });
    }

    internal TimeSpan ElapsedTime => timer?.Elapsed ?? TimeSpan.Zero;

    internal bool IsRunning { get; private set; }

    internal string ActionDescription { get; }

    internal Task Task { get; }

    internal Exception Exception { get; private set; }
}
