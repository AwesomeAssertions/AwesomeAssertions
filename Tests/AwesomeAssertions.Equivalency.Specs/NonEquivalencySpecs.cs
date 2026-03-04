using System;
using Xunit;
using Xunit.Sdk;

namespace AwesomeAssertions.Equivalency.Specs;

public class NonEquivalencySpecs
{
    [Fact]
    public void When_asserting_inequivalence_of_equal_ints_as_object_it_should_fail()
    {
        object i1 = 1;
        object i2 = 1;

        Action act = () => i1.Should().NotBeEquivalentTo(i2);

        act.Should().Throw<XunitException>();
    }

    [Fact]
    public void When_asserting_inequivalence_of_unequal_ints_as_object_it_should_succeed()
    {
        object i1 = 1;
        object i2 = 2;

        i1.Should().NotBeEquivalentTo(i2);
    }

    [Fact]
    public void When_asserting_inequivalence_of_equal_strings_as_object_it_should_fail()
    {
        object s1 = "A";
        object s2 = "A";

        Action act = () => s1.Should().NotBeEquivalentTo(s2);

        act.Should().Throw<XunitException>();
    }

    [Fact]
    public void When_asserting_inequivalence_of_unequal_strings_as_object_it_should_succeed()
    {
        object s1 = "A";
        object s2 = "B";

        s1.Should().NotBeEquivalentTo(s2);
    }

    [Fact]
    public void When_asserting_inequivalence_of_equal_classes_it_should_fail()
    {
        var o1 = new { Name = "A" };
        var o2 = new { Name = "A" };

        Action act = () => o1.Should().NotBeEquivalentTo(o2, "some {0}", "reason");

        act.Should().Throw<XunitException>().WithMessage("*some reason*");
    }

    [Fact]
    public void When_asserting_inequivalence_of_unequal_classes_it_should_succeed()
    {
        var o1 = new { Name = "A" };
        var o2 = new { Name = "B" };

        o1.Should().NotBeEquivalentTo(o2);
    }
}
