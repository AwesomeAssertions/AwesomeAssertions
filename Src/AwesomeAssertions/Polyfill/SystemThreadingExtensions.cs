#if !NET8_0_OR_GREATER
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace System.Threading;

internal static class SystemThreadingExtensions
{
    public static Task CancelAsync(this CancellationTokenSource cancellationTokenSource)
    {
        cancellationTokenSource.Cancel();
        return Task.CompletedTask;
    }
}
#endif
