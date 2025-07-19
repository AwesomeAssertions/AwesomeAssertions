using System;
using AwesomeAssertions.Common;

namespace AwesomeAssertions.Specs;

internal sealed class TestTimer : ITimer
{
    private readonly Func<TimeSpan> getElapsed;

    public TestTimer(Func<TimeSpan> getElapsed)
    {
        this.getElapsed = getElapsed;
    }

    public TimeSpan Elapsed => getElapsed();

    public void Dispose()
    {
    }
}
