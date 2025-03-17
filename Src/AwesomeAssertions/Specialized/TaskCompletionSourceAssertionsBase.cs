using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions.Common;

namespace AwesomeAssertions.Specialized;

#pragma warning disable CS0659, S1206 // Ignore not overriding Object.GetHashCode()
/// <summary>
/// Implements base functionality for assertions on TaskCompletionSource.
/// </summary>
public class TaskCompletionSourceAssertionsBase
{
    protected TaskCompletionSourceAssertionsBase(IClock clock)
    {
        Clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }

    private protected IClock Clock { get; }

    /// <inheritdoc/>
    [SuppressMessage("Design", "CA1065:Do not raise exceptions in unexpected locations")]
    public override bool Equals(object obj) =>
        throw new NotSupportedException("Equals is not part of Awesome Assertions. Did you mean CompleteWithinAsync() instead?");

    /// <summary>
    ///     Monitors the specified task whether it completes withing the remaining time span.
    /// </summary>
    private protected async Task<bool> CompletesWithinTimeoutAsync(Task target, TimeSpan remainingTime)
    {
        using var timeoutCancellationTokenSource = new CancellationTokenSource();

        Task completedTask =
            await Task.WhenAny(target, Clock.DelayAsync(remainingTime, timeoutCancellationTokenSource.Token));

        if (completedTask != target)
        {
            return false;
        }

        // cancel the clock
#pragma warning disable CA1849 // Call async methods when in an async method: Is not a drop-in replacement in this case, but may cause problems.
        timeoutCancellationTokenSource.Cancel();
#pragma warning restore CA1849 // Call async methods when in an async method
        return true;
    }
}
