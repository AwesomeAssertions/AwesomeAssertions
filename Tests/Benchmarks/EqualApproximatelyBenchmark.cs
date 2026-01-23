using System.Linq;
using AwesomeAssertions;
using BenchmarkDotNet.Attributes;

namespace Benchmarks;

[MemoryDiagnoser]
public class EqualApproximatelyBenchmark
{
    private float[] subject;
    private float[] expectationExact;
    private float[] expectationApproximate;

    [Params(1, 100, 10_000, 1_000_000)]
    public int N { get; set; }

    [GlobalSetup]
    public void GlobalSetup()
    {
        subject = [.. Enumerable.Range(0, N).Select(i => (float)i)];
        expectationExact = [.. Enumerable.Range(0, N).Select(i => (float)i)];
        expectationApproximate = [.. Enumerable.Range(0, N).Select(i => i + 0.001f)];
    }

    [Benchmark]
    public void BeEquivalentTo_UsingBeApproximately_ExactlyEqualArrays() =>
        subject.Should().BeEquivalentTo(expectationExact, options => options
            .Using<float>(ctx => ctx.Subject.Should().BeApproximately(ctx.Expectation, 0.01f))
            .WhenTypeIs<float>()
            .WithStrictOrdering());

    [Benchmark]
    public void BeEquivalentTo_UsingBeApproximately_ApproximatelyEqualArrays() =>
        subject.Should().BeEquivalentTo(expectationApproximate, options => options
            .Using<float>(ctx => ctx.Subject.Should().BeApproximately(ctx.Expectation, 0.01f))
            .WhenTypeIs<float>()
            .WithStrictOrdering());

    [Benchmark]
    public void Equal_ExactlyEqualArrays() =>
        subject.Should().Equal(expectationExact);

#if NET8_0_OR_GREATER
    [Benchmark]
    public void EqualApproximately_ExactlyEqualArrays() =>
        subject.Should().EqualApproximately(expectationExact, 0.01f);

    [Benchmark]
    public void EqualApproximately_ApproximatelyEqualArrays() =>
        subject.Should().EqualApproximately(expectationApproximate, 0.01f);
#endif
}
