using AwesomeAssertions;
using BenchmarkDotNet.Attributes;

namespace Benchmarks;

[MemoryDiagnoser]
public class NumericAssertionsBaseBenchmarks
{
    [Benchmark]
    public object Be_ValuesEqual()
        => 3d.Should().Be(3d);

#if NET
    [Benchmark]
    public object BeCloseTo_Overflow_Success()
        => System.Int128.MinValue.Should().BeCloseTo(System.Int128.MinValue + 1, 2);

    [Benchmark]
    public object BeCloseTo2_Overflow_Success()
        => System.Int128.MinValue.Should().BeCloseTo2(System.Int128.MinValue + 1, 2);

    [Benchmark]
    public object BeCloseTo_NoOverflow_Success()
        => System.Int128.One.Should().BeCloseTo(System.Int128.Zero, 2);

    [Benchmark]
    public object BeCloseTo2_NoOverflow_Success()
        => System.Int128.One.Should().BeCloseTo2(System.Int128.Zero, 2);
#endif
}
