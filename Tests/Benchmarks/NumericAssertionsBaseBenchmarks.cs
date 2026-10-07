using AwesomeAssertions;
using BenchmarkDotNet.Attributes;

namespace Benchmarks;

[MemoryDiagnoser]
public class NumericAssertionsBaseBenchmarks
{
    [Benchmark]
    public object Be_ValuesEqual()
        => 3d.Should().Be(3d);
}
