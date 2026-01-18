using System;
using Xunit;

namespace AwesomeAssertions.Equivalency.Specs;

public class TupleSpecs
{
    [Fact]
    public void When_a_nested_member_is_a_tuple_it_should_compare_its_property_for_equivalence()
    {
        var actual = new { Tuple = (new[] { "string1" }, new[] { "string2" }) };
        var expected = new { Tuple = (new[] { "string1" }, new[] { "string2" }) };

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void When_a_tuple_is_compared_it_should_compare_its_components()
    {
        var actual = Tuple.Create("Hello", true, new[] { 3, 2, 1 });
        var expected = Tuple.Create("Hello", true, new[] { 1, 2, 3 });

        actual.Should().BeEquivalentTo(expected);
    }
}
