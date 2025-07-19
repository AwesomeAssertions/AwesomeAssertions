using System.Collections.Generic;
using AwesomeAssertions.Common;
using BenchmarkDotNet.Attributes;

namespace Benchmarks;

[MemoryDiagnoser]
public class HasValueSemanticsBenchmarks
{
    [Benchmark(Baseline = true)]
    public bool HasValueSemantics_ValueType() => typeof(int).HasValueSemantics();

    [Benchmark]
    public bool HasValueSemantics_Object() => typeof(object).HasValueSemantics();

    [Benchmark]
    public bool HasValueSemantics_OverridesEquals() => typeof(string).HasValueSemantics();

    [Benchmark]
    public bool HasValueSemantics_AnonymousType() => new { }.GetType().HasValueSemantics();

    [Benchmark]
    public bool HasValueSemantics_KeyValuePair() => typeof(KeyValuePair<int, int>).HasValueSemantics();

    [Benchmark]
    public bool HasValueSemantics_ValueTuple() => typeof((int, int)).HasValueSemantics();
}
