using System.Threading;

namespace AwesomeAssertions.Events;

/// <summary>
/// Generates a sequence in a thread-safe manner.
/// </summary>
internal sealed class ThreadSafeSequenceGenerator
{
    private int sequence = -1;

    /// <summary>
    /// Increments the current sequence.
    /// </summary>
    public int Increment()
    {
        return Interlocked.Increment(ref sequence);
    }
}
