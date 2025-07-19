using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AwesomeAssertions;
using AwesomeAssertions.Collections;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmarks;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net472)]
[SimpleJob(RuntimeMoniker.Net80)]
public class Issue1657
{
    private List<ExampleObject> list;
    private List<ExampleObject> list2;

    [Params(1, 10, 50)]
    public int N { get; set; }

    [GlobalSetup]
    public void GlobalSetup()
    {
        list = Enumerable.Range(0, N).Select(i => GetObject(i)).ToList();
        list2 = Enumerable.Range(0, N).Select(i => GetObject(N - 1 - i)).ToList();
    }

    [Benchmark]
    public AndConstraint<GenericCollectionAssertions<ExampleObject>> BeEquivalentTo() =>
        list.Should().BeEquivalentTo(list2);

    private static ExampleObject GetObject(int i)
    {
        string iToString = i.ToString("D2", CultureInfo.InvariantCulture);
        return new ExampleObject
        {
            Id = iToString,
            Value1 = iToString,
            Value2 = iToString,
            Value3 = iToString,
            Value4 = iToString,
            Value5 = iToString,
            Value6 = iToString,
            Value7 = iToString,
            Value8 = iToString,
            Value9 = iToString,
            Value10 = iToString,
            Value11 = iToString,
            Value12 = iToString,
        };
    }
}

public class ExampleObject
{
    public string Id { get; set; }

    public string Value1 { get; set; }

    public string Value2 { get; set; }

    public string Value3 { get; set; }

    public string Value4 { get; set; }

    public string Value5 { get; set; }

    public string Value6 { get; set; }

    public string Value7 { get; set; }

    public string Value8 { get; set; }

    public string Value9 { get; set; }

    public string Value10 { get; set; }

    public string Value11 { get; set; }

    public string Value12 { get; set; }
}
